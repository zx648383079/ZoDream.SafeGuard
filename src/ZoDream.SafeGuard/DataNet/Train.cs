using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.SafeGuard.DataNet
{
    public class Train
    {

        private readonly string ModelPath = Path.Combine(Environment.CurrentDirectory, "data", "SdcaLogisticRegression_Model.zip");

        public void TrainAndSave()
        {
            var context = new MLContext();
            var dataPath = Path.Combine(Environment.CurrentDirectory, "data", "input.csv");
            var fullData = context.Data.LoadFromTextFile<CheckData>(path: dataPath, hasHeader: true, separatorChar: ',');
            var trainTestData = context.Data.TrainTestSplit(fullData, testFraction: .2);
            var trainData = trainTestData.TrainSet;

        }
        public void LoadAndPrediction()
        {
            var context = new MLContext();
            var model = context.Model.Load(ModelPath, out var inputSchema);
            var predictionEngine = context.Model.CreatePredictionEngine<CheckData, CheckPrediction>(model);

            var test = new CheckData
            {

            };
            var prediction = predictionEngine.Predict(test);
        }

        public ITransformer BuildAndTrainModel(MLContext context, IDataView splitTrainSet)
        {
            var estimator = context.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(CheckData.Label))
                .Append(context.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
            // 开始生成模型
            var model = estimator.Fit(splitTrainSet);
            // 生成完成
            context.Model.Save(model, splitTrainSet.Schema, ModelPath);
            return model;
        }
    }
}
