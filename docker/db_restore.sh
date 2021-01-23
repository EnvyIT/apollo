#!/bin/bash

#!/bin/bash

TEST_ROOT_PASSWORD=8b1LnIub
DEV_ROOT_PASSWORD=3a71VNaza

COLOR_RED='\033[0;31m'
COLOR_GREEN='\033[0;32m'
COLOR_NONE='\033[0m'

contains_error

validate_result() {
  return_code=$?
  argument=$1

  if [ "${return_code}" -eq 0  ]; then
    echo -e "${COLOR_GREEN}Successfully restored ${argument}${COLOR_NONE}"
  else
    echo -e "${COLOR_RED}Could not restore database ${argument}${COLOR_NONE}"
  fi
}

backup_database() {
  container=$1
  password=$2
  tar_file=$3

  set -o pipefail

  echo
  echo "Restore database ${container} ..."

  docker_command="./usr/bin/mysql -u root --password=${password} apollo"
  zcat "${tar_file}" | docker exec -i "${container}" sh -c "${docker_command}; exit $?"

  validate_result "${container}"
}

wait_end() {
  read -r -p "Press any key to continue."
}

main() {
  echo "Starting restore for Apollo-Databases ..."

  backup_database "apollo.test.db" "${TEST_ROOT_PASSWORD}" "apollo_test_backup.sql.gz"
  backup_database "apollo.dev.db" "${DEV_ROOT_PASSWORD}" "apollo_dev_backup.sql.gz"

  echo "Restore completed."
  wait_end
}

main