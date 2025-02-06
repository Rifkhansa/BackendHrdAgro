using System.IO.Compression;

namespace BackendHrdAgro.Models
{

    public class DocumentDropper
    {
        private static IConfigurationRoot ConfReader() => new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

        public static string RootDirectory(bool isProduction) => isProduction ? ConfReader().GetSection("DocumentPlacement:Production:Root").Value! : ConfReader().GetSection("DocumentPlacement:Development:Root").Value!;

        public static string SetDirectory(ModuleDirectory directory, ClosingModuleAction action, bool isProduction)
        {
            if (isProduction)
            {
                switch (action)
                {
                    case ClosingModuleAction.Root:
                        return ConfReader().GetSection("DocumentPlacement:Production:Closing:Root").Value!;
                    case ClosingModuleAction.Document:
                        return ConfReader().GetSection("DocumentPlacement:Production:Closing:Document").Value!;
                    case ClosingModuleAction.Payment:
                        return ConfReader().GetSection("DocumentPlacement:Production:Closing:Payment").Value!;
                    default:
                        return ConfReader().GetSection("DocumentPlacement:Production:Root").Value!;
                }
            }
            else
            {
                switch (action)
                {
                    case ClosingModuleAction.Root:
                        return ConfReader().GetSection("DocumentPlacement:Development:Closing:Root").Value!;
                    case ClosingModuleAction.Document:
                        return ConfReader().GetSection("DocumentPlacement:Development:Closing:Document").Value!;
                    case ClosingModuleAction.Payment:
                        return ConfReader().GetSection("DocumentPlacement:Development:Closing:Payment").Value!;
                    default:
                        return ConfReader().GetSection("DocumentPlacement:Development:Root").Value!;
                }
            }

        }

        public static string SetDirectory(ModuleDirectory directory, ClaimModuleAction action, bool isProduction)
        {
            if (isProduction)
            {
                switch (action)
                {
                    case ClaimModuleAction.Root:
                        return ConfReader().GetSection("DocumentPlacement:Production:Claim:Root").Value!;
                    case ClaimModuleAction.Document:
                        return ConfReader().GetSection("DocumentPlacement:Production:Claim:Document").Value!;
                    case ClaimModuleAction.Payment:
                        return ConfReader().GetSection("DocumentPlacement:Production:Claim:Payment").Value!;
                    case ClaimModuleAction.Reserve:
                        return ConfReader().GetSection("DocumentPlacement:Production:Claim:Reserve").Value!;
                    case ClaimModuleAction.Dla:
                        return ConfReader().GetSection("DocumentPlacement:Production:Claim:Dla").Value!;
                    case ClaimModuleAction.Subrogasi:
                        return ConfReader().GetSection("DocumentPlacement:Production:Claim:Subrogasi").Value!;
                    default:
                        return ConfReader().GetSection("DocumentPlacement:Production:Root").Value!;
                }
            }
            else
            {
                switch (action)
                {
                    case ClaimModuleAction.Root:
                        return ConfReader().GetSection("DocumentPlacement:Development:Claim:Root").Value!;
                    case ClaimModuleAction.Document:
                        return ConfReader().GetSection("DocumentPlacement:Development:Claim:Document").Value!;
                    case ClaimModuleAction.Payment:
                        return ConfReader().GetSection("DocumentPlacement:Development:Claim:Payment").Value!;
                    case ClaimModuleAction.Reserve:
                        return ConfReader().GetSection("DocumentPlacement:Development:Claim:Reserve").Value!;
                    case ClaimModuleAction.Dla:
                        return ConfReader().GetSection("DocumentPlacement:Development:Claim:Dla").Value!;
                    case ClaimModuleAction.Subrogasi:
                        return ConfReader().GetSection("DocumentPlacement:Development:Claim:Subrogasi").Value!;
                    default:
                        return ConfReader().GetSection("DocumentPlacement:Development:Root").Value!;
                }
            }

        }

        public static string SetDirectory(ModuleDirectory directory, RefundModuleAction action, bool isProduction)
        {
            if (isProduction)
            {
                switch (action)
                {
                    case RefundModuleAction.Root:
                        return ConfReader().GetSection("DocumentPlacement:Production:Refund:Root").Value!;
                    case RefundModuleAction.Document:
                        return ConfReader().GetSection("DocumentPlacement:Production:Refund:Document").Value!;
                    case RefundModuleAction.Payment:
                        return ConfReader().GetSection("DocumentPlacement:Production:Refund:Payment").Value!;
                    default:
                        return ConfReader().GetSection("DocumentPlacement:Production:Root").Value!;
                }
            }
            else
            {
                switch (action)
                {
                    case RefundModuleAction.Root:
                        return ConfReader().GetSection("DocumentPlacement:Development:Refund:Root").Value!;
                    case RefundModuleAction.Document:
                        return ConfReader().GetSection("DocumentPlacement:Development:Refund:Document").Value!;
                    case RefundModuleAction.Payment:
                        return ConfReader().GetSection("DocumentPlacement:Development:Refund:Payment").Value!;
                    default:
                        return ConfReader().GetSection("DocumentPlacement:Development:Root").Value!;
                }
            }
        }

        public static string SetDirectory(ModuleDirectory directory, HRDModuleAction action, bool isProduction)
        {
            if (isProduction)
            {
                switch (action)
                {
                    case HRDModuleAction.Root:
                        return ConfReader().GetSection("DocumentPlacement:Production:HRD:Root").Value!;
                    case HRDModuleAction.Document:
                        return ConfReader().GetSection("DocumentPlacement:Production:HRD:Document").Value!;
                    case HRDModuleAction.Termination:
                        return ConfReader().GetSection("DocumentPlacement:Production:HRD:Termination").Value!;
                    case HRDModuleAction.Permitt:
                        return ConfReader().GetSection("DocumentPlacement:Production:HRD:Permitt").Value!;
                    case HRDModuleAction.Mutasi:
                        return ConfReader().GetSection("DocumentPlacement:Production:HRD:Mutasi").Value!;
                    case HRDModuleAction.Extend:
                        return ConfReader().GetSection("DocumentPlacement:Production:HRD:Extend").Value!;
                    case HRDModuleAction.PegawaiNext:
                        return ConfReader().GetSection("DocumentPlacement:Production:HRD:PegawaiNext").Value!;
                    case HRDModuleAction.OutgoingLetter:
                        return ConfReader().GetSection("DocumentPlacement:Production:HRD:OutgoingLetter").Value!;
                    case HRDModuleAction.NoCard:
                        return ConfReader().GetSection("DocumentPlacement:Production:HRD:NoCard").Value!;
                    case HRDModuleAction.WarningLetter:
                        return ConfReader().GetSection("DocumentPlacement:Production:HRD:WarningLetter").Value!;
                    default:
                        return ConfReader().GetSection("DocumentPlacement:Production:Root").Value!;
                }
            }
            else
            {
                switch (action)
                {
                    case HRDModuleAction.Root:
                        return ConfReader().GetSection("DocumentPlacement:Development:HRD:Root").Value!;
                    case HRDModuleAction.Document:
                        return ConfReader().GetSection("DocumentPlacement:Development:HRD:Document").Value!;
                    case HRDModuleAction.Termination:
                        return ConfReader().GetSection("DocumentPlacement:Development:HRD:Termination").Value!;
                    case HRDModuleAction.Permitt:
                        return ConfReader().GetSection("DocumentPlacement:Development:HRD:Permitt").Value!;
                    case HRDModuleAction.Mutasi:
                        return ConfReader().GetSection("DocumentPlacement:Development:HRD:Mutasi").Value!;
                    case HRDModuleAction.Extend:
                        return ConfReader().GetSection("DocumentPlacement:Development:HRD:Extend").Value!;
                    case HRDModuleAction.PegawaiNext:
                        return ConfReader().GetSection("DocumentPlacement:Development:HRD:PegawaiNext").Value!;
                    case HRDModuleAction.OutgoingLetter:
                        return ConfReader().GetSection("DocumentPlacement:Development:HRD:OutgoingLetter").Value!;
                    case HRDModuleAction.NoCard:
                        return ConfReader().GetSection("DocumentPlacement:Development:HRD:NoCard").Value!;
                    case HRDModuleAction.WarningLetter:
                        return ConfReader().GetSection("DocumentPlacement:Development:HRD:WarningLetter").Value!;
                    default:
                        return ConfReader().GetSection("DocumentPlacement:Development:Root").Value!;
                }
            }

        }

        public static string? FindFileRecursively(string folderPath, string targetFileName)
        {
            foreach (string file in Directory.GetFiles(folderPath, targetFileName))
            {
                return file; // Return the first match found
            }

            foreach (string subfolder in Directory.GetDirectories(folderPath))
            {
                string targetFilePath = FindFileRecursively(subfolder, targetFileName);
                if (!string.IsNullOrEmpty(targetFilePath))
                {
                    return targetFilePath; // Return the match from the subfolder
                }
            }

            return null; // File not found
        }
    }

    public class DocumentPicker
    {
        public static ExtractAndFindExcelFileCashback ExtractAndFindExcelFile(string compressedFolder, bool isDevelopment, string extractionPath)
        {
            Directory.CreateDirectory(extractionPath);
            ExtractAndFindExcelFileCashback cashback = new ExtractAndFindExcelFileCashback();
            List<string> excel = new List<string>();
            List<string> pdf = new List<string>();
            List<string> other = new List<string>();
            using (ZipArchive archive = ZipFile.OpenRead(compressedFolder))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string destinationPath = Path.Combine(extractionPath, entry.Name);

                    if (entry.FullName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                    {
                        // This entry is an Excel file, you can process it as needed
                        Console.WriteLine($"Found xlsx file: {entry.FullName}");
                        entry.ExtractToFile(destinationPath, true);
                        excel.Add(destinationPath);
                    }
                    else if (entry.FullName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                    {
                        // Process PDF files or skip them
                        Console.WriteLine($"Found xls file: {entry.FullName}");
                        entry.ExtractToFile(destinationPath, true);
                        excel.Add(destinationPath);
                    }
                    else if (entry.FullName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        // Process PDF files or skip them
                        Console.WriteLine($"Found pdf file: {entry.FullName}");
                        entry.ExtractToFile(destinationPath, true);
                        pdf.Add(destinationPath);
                    }
                    else
                    {
                        Console.WriteLine($"Found other file: {entry.FullName}");
                        // Skip other files
                        entry.ExtractToFile(destinationPath, true);
                        other.Add(destinationPath);
                    }
                }
            }
            cashback.Excel = excel;
            cashback.Pdf = pdf;
            cashback.Other = other;
            return cashback;
        }
    }

    public class ExtractAndFindExcelFileCashback
    {
        public List<string>? Excel { get; set; }
        public List<string>? Pdf { get; set; }
        public List<string>? Other { get; set; }
    }

    public enum ModuleDirectory
    {
        Closing,
        Claim,
        Refund,
        Root,
        HRD
    }

    public enum ClaimModuleAction
    {
        Root,
        Payment,
        Document,
        Reserve,
        Dla,
        Subrogasi 
    }

    public enum ClosingModuleAction
    {
        Root,
        Payment,
        Document 

    }

    public enum RefundModuleAction
    {
        Root,
        Payment,
        Document 
    }
    public enum HRDModuleAction
    {
        Root,
        Termination,
        Document,
        Permitt,
        Mutasi,
        Extend,
        PegawaiNext,
        OutgoingLetter,
        NoCard,
        Report,
        WarningLetter
    }

}
