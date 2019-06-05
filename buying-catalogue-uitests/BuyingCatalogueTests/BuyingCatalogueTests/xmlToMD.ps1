param (
    [string]$xml = $(throw "-xml is required."),
    [string]$xsl = $(throw "-xsl is required."),
    [string]$output = $(throw "-output is required.")
)

# var = new XslCompiledTransform(true);
$xslt = New-Object -TypeName "System.Xml.Xsl.XslCompiledTransform"

# xslt.Load(stylesheet);
$xslt.Load($xsl)

# xslt.Transform(sourceFile, null, sw);
$xslt.Transform($xml, $output)