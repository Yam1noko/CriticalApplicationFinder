hostport=$1
shift
cmd="$@"

host=$(echo $hostport | cut -d: -f1)
port=$(echo $hostport | cut -d: -f2)

while ! nc -z $host $port; do
  echo "Waiting for $host:$port..."
  sleep 1
done

echo "$host:$port is available - running command"
exec $cmd
