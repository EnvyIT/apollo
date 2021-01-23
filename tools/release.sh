#!/bin/bash

# Colors
COLOR_RED='\033[0;31m'
COLOR_GREEN='\033[0;32m'
COLOR_BLUE='\033[0;36m'
COLOR_NONE='\033[0m'

# Constants
REGEX_SEMANTIC_VERSIONING="^([0-9]+\.){0,2}(\*|[0-9]+)$"
RELEASE_PATH="release"
SUP_PATH_ASSETS="assets"
TOOLS_PATH="tools"

# Terminal
TERMINAL_SOURCE="../Apollo/Apollo.Terminal"
SUP_PATH_TERMINAL="terminal"
TARGET_PLATFORM="win-x64"
TRIM_FILES_ENABLED=false
READY_TO_RUN_ENABLED=true
BAT_FILE="Apollo.bat"

# API
API_SOURCE="../Apollo/"
SUP_PATH_API="api"
API_CONTAINER_NAME="apollo.api"
DOCKERFILE_PATH="Apollo.Api/Dockerfile"
ARCHIVE_EXTENSION="tar.gz"

# Message Helper
print_error() {
  echo -e "${COLOR_RED}${1}${COLOR_NONE}"
}
print_success() {
  echo -e "${COLOR_GREEN}${1}${COLOR_NONE}"
}
print_info() {
  echo -e "${COLOR_BLUE}${1}${COLOR_NONE}"
}
print_message() {
  echo -e "${1}"
}
print_new_line(){
  echo -e ""
}


# Helper

validate_parameters() {
  if [ "$#" -lt 1 ]; then
    print_error "No version parameter given!"
    exit 1
  fi

  if [ "$#" -gt 1 ]; then
    print_error "Too many parameters given!"
    exit 2
  fi

  if [[ ! $1 =~ $REGEX_SEMANTIC_VERSIONING ]]; then
    print_error "Invalid version given (no semantic version)!"
    exit 1
  fi
}

check_return_code() {
  if [ "$1" -ne 0 ]; then
    print_error "${2}"
    exit $1
  fi
}


# Actions

cleanup() {
  print_info "Cleanup old releases ..."

  rm -rf "../${RELEASE_PATH}"
  check_return_code $? "Unable to cleanup output path!"

  mkdir -p "../${RELEASE_PATH}"

  print_success "Old releases successfully removed."
}

release_terminal() {
  print_info "Release terminal ..."

  path="../../${RELEASE_PATH}/${SUP_PATH_TERMINAL}";
  rm -rf "${path}"
  check_return_code $? "Unable to cleanup output path!"

  dotnet publish -c Release \
                -r "${TARGET_PLATFORM}" \
                -o "../../${RELEASE_PATH}/${SUP_PATH_TERMINAL}/${SUP_PATH_ASSETS}" \
                --self-contained true \
                -p:PublishReadyToRun="${READY_TO_RUN_ENABLED}" \
                -p:PublishTrimmed="${TRIM_FILES_ENABLED}"
  check_return_code $? "Release project failed!"

  cd "${path}"

  cp "../../${TOOLS_PATH}/${BAT_FILE}" "./${BAT_FILE}"
  check_return_code $? "Unable to copy start file!"

  print_success "Terminal successfully released"
}

release_api() {
  version=$1
  print_info "Release API ..."


  imageName="${API_CONTAINER_NAME}:${version}"
  docker build -t "${imageName}" -f "${DOCKERFILE_PATH}" .
  check_return_code $? "Build API container failed!"

  path="../${RELEASE_PATH}/${SUP_PATH_API}"
  mkdir -p "${path}"

  docker save "${imageName}" -o "${path}/${API_CONTAINER_NAME}-${version}.${ARCHIVE_EXTENSION}"
  check_return_code $? "Export image failed!"

  print_success "API successfully released."
}

create_archive() {
  print_info "Create release archive ..."

  tar -czf "${1}.${ARCHIVE_EXTENSION}" "${SUP_PATH_TERMINAL}" "${SUP_PATH_API}"
  check_return_code $? "Create archive failed!"

  print_success "Archive successfully created."
}


# Main

main() {
  print_message "Release tool!"
  print_new_line

  validate_parameters "$@"

  cleanup
  print_new_line

  version=$1
  print_info "Release version ${version} ..."
  print_new_line

  (cd "${TERMINAL_SOURCE}" && release_terminal "${version}")
  print_new_line

  (cd "${API_SOURCE}" && release_api "${version}")
  print_new_line

  (cd "../${RELEASE_PATH}" && create_archive "apollo-${version}")
  print_new_line

  print_success "Version ${version} successfully released."
}

main "$@"