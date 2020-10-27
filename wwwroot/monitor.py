#!/usr/bin/env python

# monitor.py
# 2016-09-17
# Public Domain

# monitor.py          # monitor all GPIO
# monitor.py 23 24 25 # monitor GPIO 23, 24, and 25

import sys
import time
import pigpio
import requests
import threading

pigpio.exceptions = True

counter = [None]*32
cb = {}


class RequestThread(threading.Thread):
    def __init__(self, url, data, headers):
        super(RequestThread, self).__init__()
        self.url = url
        self.data = data
        self.headers = headers

    def run(self):
        try:
            response = requests.post(
                url=self.url, json=self.data, headers=self.headers, timeout=5)
            print(response.status_code, response.reason)
        except:
            pass


def cbf(g, level, tick):
    counter[g] = tick
    cb[g].cancel()

    if counter[g] is not None and level == 1:
        counter[g] += 1

    if g in cb:
        print("G={} l={} counter={}".format(
            g, level, counter[g]))

        url = "http://localhost:5000/service-center/update"
        payload = {"port": g, "value": level}
        headers = {'Content-type': 'application/json', 'Accept': 'text/plain'}
        thread = RequestThread(url, payload, headers)
        thread.start()

    time.sleep(1000)
    cb[g] = pi.callback(g, pigpio.EITHER_EDGE, cbf)


pi = pigpio.pi()
if not pi.connected:
    exit()

if len(sys.argv) == 1:
    G = range(2, 27)
else:
    G = []
    for a in sys.argv[1:]:
        G.append(int(a))

for g in G:
    pi.set_mode(g, pigpio.INPUT)
    pi.set_glitch_filter(g, 300000)
    pi.set_pull_up_down(g, pigpio.PUD_UP)
    cb[g] = pi.callback(g, pigpio.EITHER_EDGE, cbf)

try:
    print("START")
    while True:
        time.sleep(60)
except KeyboardInterrupt:
    print("STOP")
    for g in cb:
        cb[g].cancel()

pi.stop()
