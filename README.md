[![Build status](https://ci.appveyor.com/api/projects/status/github/programmersdigest/MT940Parser?branch=master&svg=true)](https://ci.appveyor.com/api/projects/status/github/programmersdigest/MT940Parser?branch=master&svg=true)
# MT940Parser
A parser for the SWIFT MT940/MT942 format
# Changes in this fork:
1.Added support of encodings (default - utf-8-BOM) when parsing the file: ```string path="some_path"; string encoding="windows-1250"; var parser = new Parser(path, encoding);```
2.Added convertion of line termination symbols to CRLF, CRLF removed from string search (reader.ReadTo, reader.Find)
3.Added Trim and multispace delition to avoid excessive whitespace characters in field values
4.Added support of specific non digit subfields in :86: field and custom separator (any special character or none, if none all field is added into information.operationDescription)
5.Unstructured data removed from information.cs, format detection removed, all data in :86: is considered to be structured with identification of subfields
6.Added more subfields into additionalInfo parser for :86: field
7.Structure of MT940 can be now more flexible, some less important fields can be skipped and not considered to be mandatory
8.Sequence number is deleted, due to :28C: can contain not only integers but also separators (more than 1) - it is better to keep field as string
9.Added new fields to infromation, statement and other classes to get more information from :86: and :61: fields
10.Amount parser is improved to handle variable number of digits and separators (. or ,) length up to 15 in total, culture invariant
11.Date parser is improved to use native DateTime.TryParseExact with specified multiple possible date time string formats (8 and 6 digit dates yyyyMMdd yyMMdd), culture invariant

## Description
This project implements a parser for the *SWIFT MT940* and *MT942* formats used for electronic banking (e.g. for electronic statements). The result is parsed into a developer friendly object model for further processing or storing.

**Do note**: This project is part of a small housekeeping book I am currently implementing for my own personal use. It may therefore be incomplete and/or faulty. If you have corrections or suggestions, please let me know.

## Features
- Parses files in the SWIFT MT940 and MT942 formats (see Relevant Materials)
- Fast: 100.000 max-length statements in ca. 6 secs
- Low memory-footprint: streams input data and provides results on a per-statement basis (using IEnumerable)
- Should adhere to the specification
- Parsing errors will currently result in exceptions (which may be subject to change in the near future)
- Is largely unit tested (200+ tests)

## Usage
Grab the latest version from NuGet https://www.nuget.org/packages/programmersdigest.MT940Parser

```
// Just provide a file path...
using (var parser = new Parser(path)) {
    foreach (var statement in parser.Parse()) {
        // Do something
    }
}

// ...or a stream
using (var parser = new Parser(networkStream)) {
    foreach (var statement in parser.Parse()) {
        // Do something
    }
}
```

## Todos
- Parse special fields used in MT942 only (i.E. :34F:, :13D:, :90D:, :90C:)
- Provide Transaction Type ID Code (in field :61:) as enum
- Store parsing errors in each statement so that subsequent statements can still be processed
- Add comments on public members

## Relevant Materials
https://deutschebank.nl/nl/docs/MT94042_EN.pdf  
https://www.bexio.com/files/content/SEO-lp/MT940/ZKB-MT940.pdf (GER)  
https://www.kontopruef.de/mt940s.shtml (GER)  
http://www.ebics.de/spezifikation/dfue-abkommen-anlage-3-formatstandards/ (see "Appendix 3 Data Formats V3.1.pdf" ch. 8)
https://www.handelsbanken.se/shb/inet/icentsv.nsf/vlookuppics/a_filmformatbeskrivningar_fil_mt940_account_statement_20081212/$file/mt940_account_statement.pdf (especially definition for structured data in field :86:)
