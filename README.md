# Implementation of HERA (Highly Efficient Repeat Assembly)

## How to Run and Build

To build and run our implementation, follow these steps:

1. Make sure you have the .NET SDK installed on your system.

2. Clone the HERA repository to your local machine.

3. Open a command prompt or terminal and navigate to the root directory of the cloned repository.

4. Run the following command to build HERA:

   ```shell
   dotnet build
   ```

   This will automatically restore dependencies and compile the source code and generate the necessary binaries.

5. Once the build process is complete, you can run HERA using the following command:

   Replace `[options]` with the desired command-line options and arguments. For example, to use the `improve` command,
   you can run:

   ```shell
   dotnet run --project Cli improve [options]
   ```

## Example Usage

Running with no arguments will print the help text:

   ```shell
   dotnet run --project Cli improve
   USAGE
  dotnet Cli.dll improve <outputpath> --reads <value> --contigs <value> --rr-overlaps <value> --cr-overlaps <value> [options]

DESCRIPTION
  Improves contigis joined by repetitive reads.

PARAMETERS
* outputpath        The path to output file. 

OPTIONS
* -r|--reads        The path reads file. 
* -c|--contigs      The path to contigs file. 
* --rr-overlaps     The path to read-read overlaps file. 
* --cr-overlaps     The path to contig-read overlaps file. 
  -v|--verbose      Prints the output to console. Default: "False".
  --monte-carlo-repeats  The number of monte carlo repeats. Default: "1000".
  --random-seed     The random seed. Default is current time in milliseconds. Default: "63".
  --min-contig-overlap  The minimum overlap. Default: "2500".
  --min-sequence-identity  The minimum sequence identity. Default: "0.8".
  --group-size-min-difference  The group size min difference. Default: "10000".
  --group-size-window  The group size window. Default: "1000".
  -h|--help         Shows help text. 
   ```

Command can also be used as executable if given right permissions(chmod +x). Navigate
to `project_root/Cli/bin/Debug/net7.0/`

Example usage with arguments:
   ```shell
  ./Cli improve "./output.fasta" --rr-overlaps "overlapsRR.paf" --cr-overlaps "overlapsCR.paf" -r "ecoli_test_reads.fasta" -c "ecoli_test_contigs.fasta"
   ```