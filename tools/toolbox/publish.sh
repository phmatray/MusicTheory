#!/usr/bin/env bash
# Publishes the Toolbox (Blazor WebAssembly) into <site dir>/toolbox.
# Usage: publish.sh <repo root> <site dir>      Run it after SiteGen, which wipes the site dir first.
set -euo pipefail

root=${1:?usage: publish.sh <repo root> <site dir>}
site=${2:?usage: publish.sh <repo root> <site dir>}
out="$site/toolbox"
tmp=$(mktemp -d)
trap 'rm -rf "$tmp"' EXIT

dotnet publish "$root/MusicTheory.Toolbox" -c Release -o "$tmp"

rm -rf "$out"
mkdir -p "$out"
cp -R "$tmp/wwwroot/." "$out/"

# Source keeps <base href="/" /> so `dotnet run` works at the root; Pages serves under /MusicTheory/toolbox/.
if ! grep -q '<base href="/" />' "$out/index.html"; then
  echo "publish.sh: index.html has no <base href=\"/\" /> to rewrite" >&2
  exit 1
fi
# BSD and GNU sed disagree on -i, so write through a temp file.
sed 's|<base href="/" />|<base href="/MusicTheory/toolbox/" />|' "$out/index.html" > "$tmp/index.html"
cp "$tmp/index.html" "$out/index.html"
rm -f "$out/index.html.br" "$out/index.html.gz"   # precompressed copies of the pre-rewrite file
touch "$out/.nojekyll"

grep -q '<base href="/MusicTheory/toolbox/" />' "$out/index.html" \
  || { echo "publish.sh: base href rewrite did not happen" >&2; exit 1; }
[ -f "$out/_framework/blazor.webassembly.js" ] \
  || { echo "publish.sh: _framework/blazor.webassembly.js is missing" >&2; exit 1; }
echo "Toolbox published to $out"
