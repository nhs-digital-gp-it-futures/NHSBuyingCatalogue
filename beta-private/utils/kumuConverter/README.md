# Kumu to Catalogue

This python script can be used to convert the data file used to generate Kumu into the tsv's and csv's used in the Buying catalogue and the CRM.

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