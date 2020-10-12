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

last = [None]*32
cb = []


class RequestThread(threading.Thread):
    def __init__(self, url, data, headers):
        super(RequestThread, self).__init__()
        self.url = url
        self.data = data
        self.headers = headers

    def run(self):
        try:
            response = requests.post(
                url=self.url, data=self.data, headers=self.headers, timeout=5)
            print(response.status_code, response.reason)
        except:
            pass


def cbf(GPIO, level, tick):
    last[GPIO] = tick
    if last[GPIO] is not None:
        diff = pigpio.tickDiff(last[GPIO], tick)
        print("G={} l={} d={} tick={}".format(
            GPIO, level, diff, pi.get_current_tick()))

        url = "http://192.168.43.140/gpio"
        payload = {"port": GPIO, "value": level}
        headers = {"Content-type": "application/x-www-form-urlencoded",
                   "Accept": "text/plain"}
        thread = RequestThread(url, payload, headers)
        thread.start()


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
    pi.set_glitch_filter(g, 1000)
    pi.set_pull_up_down(g, pigpio.PUD_UP)
    cb.append(pi.callback(g, pigpio.EITHER_EDGE, cbf))

try:
    print("START")
    while True:
        time.sleep(60)
except KeyboardInterrupt:
    print("STOP")
    for c in cb:
        c.cancel()

pi.stop()
