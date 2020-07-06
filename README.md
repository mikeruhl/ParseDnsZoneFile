# Parse Dns Zone File

### Why?
I needed to parse a DNS Zone File in Azure.  I needed to find unique destinations so I could catalog all of the places we need certificates.

### How?
1. Install [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest)
2. Run this after logging into the cli (`az login`): `az network dns zone export -g dnszone-rg -n yourdomain.com  -f output.txt` with your own values for -g (resource group), -n (dns zone name, your domain most likely), and -f (output file).
3. Compile and run the included ParseDnsZone project.  It is .net core so it's cross-plat form.  Just make your only argument the qualified path of the output file from step 2.

### And then?
You get 2 output files, output-list.csv and output-hashes.csv.  The name of the file is the same as the input file with different extensions.  So if your input was "c:\temp\bakery.txt", your output files would be "c:\temp\bakery-list.csv" and "c:\temp\bakery-hashes.csv".

Of course you can go into the code and change it to whatever output you want.  This app isn't rocket science.

In the -list.csv file, you get a better representation of what was in the original DNS Zone file, it's just now in csv and is better machine readble.  It comes with headers:  name, type, ttl, and value.  The -hashed.csv file is an output keyed on the record value with a comma-separated list of record names in the second column.

### Support?
I wrote this in 5 minutes just to spit out some junk.  If you need the same junk spit out, feel free to use this.  I do not plan on supporting this code beyond today, just thought it would be nice to slap it online for any other fine folks looking to do the same thing. But if you have questions, open an issue and I'll respond!
