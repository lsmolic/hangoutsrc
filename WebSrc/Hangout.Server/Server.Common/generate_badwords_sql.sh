#!/bin/bash

cat badwords.txt | xargs -I {} echo "INSERT INTO \`HangoutLoggingDatabase\`.\`NaughtyWords\` (\`WordToReplace\`) VALUES ('{}');"
