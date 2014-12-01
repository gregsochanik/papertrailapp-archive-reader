papertrailapp-archive-reader
============================

A console app that allows you to download a range of archives, unpack and filter them. 
Outputting the results to whatever stream you wish, currently defaults to a file.

Requires an environment var (`PAPERTRAILAPP_API_KEY`) to contain your papertrailapp api key

Current parameters
------------------

* `/Q` Query - Only rows containing this string are returned from the archive
* `/O` OutputFile - Where the file is saved to
* `/F` From - ISO 86701 datetime string indicating when you want to download archives from - REQUIRED
* `/T` To - ISO 86701 datetime string indicating when you want to download archives to (inclusive)
* `/D` Days - Number of days you want to download
* `/V` Verbose - True/False True=Write full rows to output, False=write rowcount grouped by day to output

Roadmap
-------
* Batched parallel download of archives
* Ability to merge all data into 1 stream 
* Ability to rollup count by day or output all logs
* Regex filtering support
* Allow plugins to write streams to.
