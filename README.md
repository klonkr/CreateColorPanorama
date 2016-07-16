# CreateColorPanorama
A simlpe command line tool to create panoramas based on a movies' color theme. It spits out an SVG in panoramas/
all the screen caps in output/movie/.

You need to download both ffmpeg.exe and ffprobe.exe, available at: https://ffmpeg.org/download.html#build-windows
This need to be in bin/

There are a few diffirent arguments available:

- -i  Specifies the input video file, **required**
- -w  Specifies the width of each individual vertical line, optional, default is 1
- -h  Specifies the height of the resulting svg
- -s  Specifies the Seconds per pic, aka, how many seconds between each screen cap
- -o  Specifies the output file name

In progress:
* Input a dir with pictures and create a colorpanorama from this
