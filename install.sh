#!/bin/sh

gforthmi --application hc800dump hc800appl.fs
mkdir -p $HOME/.local/bin
cp hc800dump $HOME/.local/bin
