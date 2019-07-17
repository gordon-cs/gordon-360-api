#!/bin/bash

# 2019-06-28 <jonathan.senning@gordon.edu>
#
# Grabs API routes from the API controllers and displays them in a list.
# Route prefixes are indented 4 spaces and subroutes are indented 8 spaces.
# Actual routes are composed of a prefix followed by a subroute.

find ./Gordon360/ApiControllers/ -type f -exec grep -H '\[Route' {} \; |\
    awk '{print $2,$3,$4,$5}' |\
    sed -e 's/\[RoutePrefix(/    &/g' -e 's/\[Route(/        &/g' -e 's/ *$//g'
