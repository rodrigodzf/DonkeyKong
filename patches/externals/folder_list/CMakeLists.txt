cmake_minimum_required(VERSION 2.8)

set(CMAKE_SUPPRESS_REGENERATION true)
set(CMAKE_MACOSX_RPATH Off)
set(CMAKE_OSX_DEPLOYMENT_TARGET 10.4)
set(CMAKE_OSX_ARCHITECTURES "i386;x86_64")

include(pd.build/pd.cmake)

project(folder_list)
set_pd_sources(${PROJECT_SOURCE_DIR}/pure-data/src/)
set_pd_external_path("${PROJECT_SOURCE_DIR}/build/")

add_pd_external(folder_list folder_list ${PROJECT_SOURCE_DIR}/folder_list.c)
