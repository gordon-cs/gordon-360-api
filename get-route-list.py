#!/usr/bin/env python3

"""Prints list of API routes to standard output.

Usage:
    [python3] get-route-list.py [FILE_LIST]

Finds all routes in the API controller source files.  The route type
('HttpGet', 'HttpPut', etc.) must appear BEFORE the 'Route(...)' statement.

FILE_LIST, if supplied, is a whitespace-delimited list of controller file
paths.  If not supplied then all controller files in Gordon360/ApiControllers/
directory will be used.
"""

def findRoutes(controllerFileName, tag="HttpGet"):
    """Returns list of routes matching specified type.

    Args:
        controllerFileName (str): name of controller source file.
        tag (str): route type.

    Returns:
        list of str: list of routes that match the specfied type.
    """
    with open(controllerFileName, "r") as controllerFile:
        text = controllerFile.read()
    routePrefix = "/" + getRoutePrefix(text)
    routes = []
    tagStart = text.find(tag)
    while tagStart >= 0:
        routeStart = text.find("[Route", tagStart+1)
        if routeStart >= 0:
            routeEnd = text.find("]", routeStart+1)
            route = getDoubleQuotedText(text[routeStart:routeEnd+1])
            routes.append(f"      {routePrefix}/{route}")
        tagStart = text.find(tag, routeStart+1)
    return routes

def getRoutePrefix(text):
    """Return the contents of RoutePrefix() stored in text."""
    start = text.find("[RoutePrefix")
    end = text.find("]", start+1)
    return getDoubleQuotedText(text[start:end+1])

def getDoubleQuotedText(text):
    """Return first substring of text delimited by double quotes."""
    start = text.find('"')
    end = text.find('"', start+1)
    return text[start+1:end]

import os, sys
if __name__ == "__main__":
    fileList = sys.argv[1:]
    if len(fileList) == 0:
        controllerDir = "Gordon360/ApiControllers/"
        dirList = os.listdir(controllerDir)
        dirList.sort()
        fileList = [f"{controllerDir}{f}" for f in dirList]
    for controllerFileName in fileList:
        #print(f"\x1b[31m\x1b[1m{os.path.basename(controllerFileName)}\x1b[0m")
        print(os.path.basename(controllerFileName))
        for tag in 'HttpGet', 'HttpPut', 'HttpPost', 'HttpDelete':
            routes = findRoutes(controllerFileName, tag=tag)
            if len(routes) > 0:
                #print(f"\x1b[32m    {tag}\x1b[0m")
                print(f"    {tag}")
                for route in routes:
                    print(route)

