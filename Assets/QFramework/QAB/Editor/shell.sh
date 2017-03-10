#alias zip1='zip -x "*/\.*" -x "\.*"'
#open $2
cd $2/..
temp=$2
base=`basename $temp`
echo $base
zip -r $1 $base -X -x "*.meta"