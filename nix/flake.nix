{
  description = ".NET project template";

  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable";
    utils.url = "github:numtide/flake-utils";
  };

  outputs = inputs@{ nixpkgs, ... }:
    inputs.utils.lib.eachDefaultSystem (system:
      let
        pkgs = import nixpkgs { inherit system; };
        xorgLibs = with pkgs.xorg; [
          libICE
          libSM
          libX11
          libX11.dev
        ];
      in
      rec {
        # `nix develop`
        devShells.default = with pkgs; mkShell {
          buildInputs = [
            dotnet-sdk_8
            fontconfig
            gnumake
            icu
            openssl
          ] ++ xorgLibs;

          shellHook = ''
            export DOTNET_ROOT=${dotnet-sdk}
            export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:${lib.makeLibraryPath ([ fontconfig icu openssl ] ++ xorgLibs) }
          '';
        };
      });
}