#!/bin/bash

TEST_ROOT_PASSWORD=8b1LnIub
DEV_ROOT_PASSWORD=3a71VNaza

COLOR_RED='\033[0;31m'
COLOR_GREEN='\033[0;32m'
COLOR_NONE='\033[0m'

validate_result() {
  return_code=$?
  argument=$1

  if [ "${return_code}" -eq 0 ]
  then
    echo -e "${COLOR_GREEN}Successfully created backup for ${argument}${COLOR_NONE}"
  else
    echo -e "${COLOR_RED}Could not backup ${argument}${COLOR_NONE}"
  fi
}

backup_database() {
  container=$1
  password=$2
  tar_file=$3

  set -o pipefail

  echo
  echo "Backup for container ${container} ..."

  docker_command="./usr/bin/mysqldump --compress -u root --password=${password} apollo"
  docker exec "${container}" sh -c "${docker_command}; exit $?" | gzip -c > "${tar_file}"

  validate_result "${container}"
}

wait_end() {
  read -r -p "Press any key to continue."
}

main() {
  echo "Starting backup for Apollo-Databases ..."

  backup_database "apollo.test.db" "${TEST_ROOT_PASSWORD}" "apollo_test_backup_.sql.gz"
  backup_database "apollo.dev.db" "${DEV_ROOT_PASSWORD}" "apollo_dev_backup_.sql.gz"

  echo "Backup completed."
  wait_end
}

main