using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.SafeGuard.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.SafeGuard.DataNet
{
    public class Predict
    {
        public Predict()
        {
            _context = new MLContext();
            var model = _context.Model.Load(Train.ModelPath, out var inputSchema);
            _engine = _context.Model.CreatePredictionEngine<CheckData, CheckPrediction>(model);
        }

        private readonly MLContext _context;
        private readonly PredictionEngine<CheckData, CheckPrediction> _engine;

        public FileCheckStatus Prediction(FileInfo file)
        {
            var test = new CheckData()
            {
                Extension = file.Extension.Length > 0 ? file.Extension[1..] : string.Empty,
                Text = LocationStorage.ReadAsync(file.FullName).GetAwaiter().GetResult()
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
