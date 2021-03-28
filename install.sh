#!/bin/sh

DESTDIR=${1:-$HOME/.local/bin} 

gforthmi --application hc800dump hc800appl.fs
mkdir -p $DESTDIR
cp hc800dump $DESTDIR
