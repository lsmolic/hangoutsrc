#!/bin/bash

cat badword_exclusions.txt | xargs -I {} echo "INSERT INTO \`HangoutLoggingDatabase\`.\`NiceWords\` (\`WordToKeep\`) VALUES ('{}');"
