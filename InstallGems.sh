#!/bin/bash
if ! which rake > /dev/null; then
    echo "*** Installing Rake"
    gem install rake --no-rdoc --no-ri
fi

echo "*** Installing RubyZip"
gem install rubyzip --no-rdoc --no-ri

echo "*** Installing Albacore (build support tools)"
gem install albacore --no-rdoc --no-ri
