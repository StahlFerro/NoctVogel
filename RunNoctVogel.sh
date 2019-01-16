until ./NoctVogel; do
    echo "NoctVogel crashed with exit code $?. Respawning..." >&2
    sleep 1
done
