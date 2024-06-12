using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.CodeScanner
{
    public class CodePrediction : IMLPredict
    {
        public CodePrediction()
        {
            _context = new MLContext();
            var model = _context.Model.Load(Train.ModelPath, out var inputSchema);
            _engine = _context.Model.CreatePredictionEngine<CheckData, CheckPrediction>(model);
        }

        private readonly MLContext _context;
        private readonly PredictionEngine<CheckData, CheckPrediction> _engine;

        public FileCheckStatus Predict(FileInfo file)
        {
            var test = new CheckData()
            {
                Extension = StorageFinder.GetExtension(file),
                Text = LocationStorage.ReadAsync(file.FullName).GetAwaiter().GetResult()
            };
            var prediction = _engine.Predict(test);
            if (Enum.TryParse<FileCheckStatus>(prediction.PredictedLabel, out var result))
            {
                return result;
            }
            return FileCheckStatus.Pass;
        }

        public FileCheckStatus Predict(string fileName, string text)
        {
            var test = new CheckData()
            {
                Extension = StorageFinder.GetExtension(fileName),
                Text = text
            };
            var prediction = _engine.Predict(test);
            if (Enum.TryParse<FileCheckStatus>(prediction.PredictedLabel, out var result))
            {
                return result;
            }
            return FileCheckStatus.Pass;
        }
    }
}
