# CreateColorPanorama
A simlpe command line tool to create panoramas based on a movies' color theme. It spits out an SVG in panoramas/
all the screen caps in output/movie/.

You need to download both ffmpeg.exe and ffprobe.exe, available at: https://ffmpeg.org/download.html#build-windows
Copy these to bin/

There are a few diffirent arguments available:

- -i  Specifies the input video file, **required**
- -w  Specifies the width of each individual vertical line, optional, default is 1
- -h  Specifies the height of the resulting svg
- -s  Specifies the Seconds per pic, aka, how many seconds between each screen cap
- -o  Specifies the output file name

In progress:
* Input a dir with pictures and create a colorpanorama from this

Thanks to [DanielSWolf](https://gist.github.com/DanielSWolf) for the progressbar code

Sample output:

*Batman v Superman, -w 1 -h 1000 -s 1, so 1 screen cap for each second, for a total of 9k vertical lines.*
![alt text](http://i.imgur.com/fcahKzV.png "Batman v Superman")

*Mr Robot s02e01, same settings as above*
![alt text](http://i.imgur.com/m7tsicj.png "Mr Robot s02e01")

*Mr Robot s02e02, same settings as above*
![alt text](http://i.imgur.com/C1RnLvl.png "Mr Robot s02e02")


