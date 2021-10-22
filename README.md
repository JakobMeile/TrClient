# TrClient
Client App for Transkribus HTR Platform

## Introduction

This is a supplementary app that makes life a bit easier when working with the Transkribus HTR Platform (https://readcoop.eu/).
The main focuses have been to automate tedious tasks and to make some batch processing of all pages in a document.

TrClient communicates directly with the Transkribus servers: All changes can be uploaded and viewed at once in other client apps.

NB: To use this app, you need to have an account at Transkribus - and of course some documents uploaded at their server.

## Highlighted features

### Filter lines and add structural tags based on:

* page number
* regular expression
* existing structural tag
* text size factor
* text length (# chars)
* position on page

### Renumber textlines logically 

(works better than Transkribus' own attempt in version 1.17!)

### Minor features

* Automatically fix regions and lines with coordinates outside page
* Fix erroneous baselines (eg. partially reversed direction, steep angles etc.)
* Simplify bounding boxes (= textline area)
* Find & replace text
* Convert tables to regions
* Set top-level region on all pages

## Version history

1.0.0.1:	21 Oct 2021:	Initial commit (though development started in July 2019)



