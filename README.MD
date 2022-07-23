# UnicornNet

Alternative Unicorn bindings because the original did not work and I don't know F#.

## Included libraries

For your convenience, I have included the following prebuild unicorn libraries.

| Platform | Arch | Version | Hash | From |
|-|-|-|-|-|
| MacOS | Arm64 | 2.0.0 | 3d3aa7b47ac52be3dfab85288fbb28048052783f  arm64/unicorn.dylib | See below |
| Linux | Arm64 | 2.0.0 | b5d0cff3183a745922e972966cc4e185569c6dba  arm64/unicorn.so | [ubuntu-cmake-aarch64.7z](https://github.com/unicorn-engine/unicorn/suites/7260858115/artifacts/292003872) |
| MacOS | x64 | 2.0.0 | 6b5fd573fe78387d38736a0b81545ee80eccaa7a  x64/unicorn.dylib | [macos-cmake-shared-x64.7z](https://github.com/unicorn-engine/unicorn/suites/7260858115/artifacts/292003868) |
| Linux | x64 | 2.0.0 | d2e09e5a8165c491e7acb7e7e9f2b053d237fb8e  x64/unicorn.so | [ubuntu-cmake-shared-x64.7z](https://github.com/unicorn-engine/unicorn/suites/7260858115/artifacts/292003875) |
| Windows | x64 | 2.0.0 | 6a840a5e8182ead5441d5c29c96f58a129d3a9fe  x64/unicorn.dll | [windows_msvc64_shared.7z](https://github.com/unicorn-engine/unicorn/suites/7260858115/artifacts/292003888) |

All binaries except `MacOS Arm64` are downloaded from https://github.com/unicorn-engine/unicorn/actions/runs/2630763111. There are no MacOS Arm64 builders on GitHub, which is why you need to build it yourself. I have included my own copy along with [PR #1648](https://github.com/unicorn-engine/unicorn/pull/1648) to fix unicorn in .NET 6.