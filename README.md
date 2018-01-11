# VoidBattery

Adjusts keyboard and mouse LEDs to indicate Void Pro headset battery level. I should preface this by stating that I have ~~no~~ some idea what I'm doing.

Huge credit and thanks to Zazzmatazz for help figuring out packet format! Seriously, this would not be possible without him, a voltmeter, and a dream. Definitely not on such a short timescale.

Thanks to Zenairo of the [RGB.Net](https://github.com/DarthAffe/RGB.NET) dev discord for packet-snooping tips.

#### Limitations:

- Does not update keyboard/mouse LEDs yet (soon™)
- Supports only the Void Pro, as of yet
- Only tested on Windows 10, version 1607
- I have no idea whether specific drivers are required by HidSharp
- I have no plans to expand this project or make it portable, as of yet

#### What it does (in the background):

1. Listens for an interrupt packet from the headset
2. Determines whether the packet is relevant (see below)
3. `&`-masks the third byte by `0x7F` to strip boom flag
4. Takes an average of the last 5 values (at most) and uses that accordingly

#### Relevant packet format (as far as I can tell):

`xx ii bb uu xx`

- `xx`: delimiter (first byte `0x64`, last byte `0x01`, always)
- `ii`: control indicator
  - Volume rocker up/down: `0x01`/`0x02`
  - Mute button down: `0x02`
  - Power button down: hell if I know
- `bb`: battery info. Uses the first bit as a flag for whether the mic boom is up or down, so we strip it (`&= 0x7F`)
- `uu`: unknown. As of yet.

#### Why.

Corsair's CUE SDK doesn't support battery level for wireless peripherals yet (and probably never will, given CUE is aimed at manipulating LEDs, not battery level). So, this happened.