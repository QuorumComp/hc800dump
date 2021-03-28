# hc800dump

hc800dump is a small utility for interpreting and display the internal structure of HC800 executable images, licensed under GPLv3

# Installing

## Building from source

### Linux and macOS

Prequisties are "gforth". It can most likely be installed with your favorite package manager.

A script (```install.sh```) is included that will install the compiled binaries into the ```$HOME/.local/bin``` directory. This path should be added to your $PATH for easier use. This script will also accept the destination root (for instance ```/usr/local/bin```).

For even easier installation, provided you have the necessary prerequisites, ```git``` and ```gforth```, installed, the latest version can be installed using

```
    curl https://raw.githubusercontent.com/QuorumComp/hc800dump/main/install.sh | sh
```

If you want to install it globally, you can supply the installation prefix as a parameter:

```
    curl https://raw.githubusercontent.com/QuorumComp/hc800dump/main/install.sh | sh -s /usr/local/bin
```
