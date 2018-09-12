let fs = require("fs"),
    path = require("path"),
    inline = require("inline-css");

const TEST_RESULTS_DIRECTORY = "./results/coverage";
const CODE_COVERAGE_DIRECTORY = "./results/coverage";

process_directory(CODE_COVERAGE_DIRECTORY)

function process_directory(scanDir) {
    fs.readdir(scanDir, (err, files)=> {
        if(err) { throw new Error(err); }

        let reports = files.filter((report)=> {
            return report.endsWith(".html");
        });

        reports.forEach((report)=> {
            let filePath = path.join(scanDir, report);
            let options = { 
                url: "file://" + path.resolve(filePath),
                extraCss: ".pad1 { padding: 0; }"
            };

            fs.readFile(path.resolve(filePath), 'utf8', (err, data)=> {
                console.log('Processing', filePath)
                inline(data, options)
                    .then((html)=> {
                        let outputFile = path.join(scanDir, report);
                        fs.writeFile(outputFile, html, (err)=> {
                            if(err) { throw err; }
                        });
                    })
                    .catch((err)=> {
                        console.log(err);
                    });
            });
        });

        files.forEach(filename => {
            const filePath = path.join(scanDir, filename)
            if (fs.statSync(filePath).isDirectory()) {
                process_directory(filePath)
            }
        })
    });
}
