import requests
import os
import time
import datetime

url = os.environ.get("QUERY_SERVICE_URL", "http://smt-query-service:8080")
interval = int(os.environ.get("SIMULATE_INTERVAL_SECONDS", "10"))
locations = os.environ.get("LOCATIONS", "loc-001").split(",")

print("SMT IoT Simulator started", flush=True)

while True:
    for loc in locations:
        now = datetime.datetime.utcnow()
        frm = (now - datetime.timedelta(hours=1)).strftime("%Y-%m-%dT%H:%M:%SZ")
        to = now.strftime("%Y-%m-%dT%H:%M:%SZ")
        try:
            r = requests.get(
                f"{url}/metrics",
                params={"locationId": loc, "from": frm, "to": to},
                timeout=5,
            )
            print(f"[{now}] location={loc} status={r.status_code} metrics={len(r.json())}", flush=True)
        except Exception as e:
            print(f"[{now}] ERROR {loc}: {e}", flush=True)
    time.sleep(interval)