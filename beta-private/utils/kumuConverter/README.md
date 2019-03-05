# Kumu to Catalogue

This python script can be used to convert the data file used to generate Kumu into the tsv's and csv's used in the Buying catalogue and the CRM.

you can run this program using python:

```
    python kumu_converter.py
```

though it expects at least one command line argument providing the file to be used during conversion.

## Command Line Arguments

The script supports a couple of command line arguments.

```
kumu_converter.py -i <input-file-path> -c <capability-output-file> -s <standards-output-file> -m <mapping-output-file> -d <delimiter>

-i, --input              is required and specifies what file is being input to the program.
-c, --capability-output  can be used to name the file that capabilities are output to.
-s, --standard-output    can be used to name the file that standards are output to.
-m, --mapping-output     can be used to name the file that capability-standard mappings are output to.
-d, --delimiter          can be used to customise the delimiter used in output files.
```

## Dependencies.

### Kumu File
This program needs a kumu file that is in the expected format.

The file must have 2 sheets: `Elements`, and `Connections`

The `Elements` file needs to have the following headings:
```
    ID
    Label
    Version
    Type
    URL
    Description
    Capability Type
    Tags
    Capability Specific Standard 1
    Capability Specific Standard URL 1
    Capability Specific Standard 2
    Capability Specific Standard URL 2
```

The `Connections` file needs to have the following headings:
```
    From
    To
    FromID
    ToID
    Connection Type
    Type
```

### openpyxl

This application relies on the openpyxl package.

This could be installed using `pip3`

```
    pip3 install openpyxl
```

