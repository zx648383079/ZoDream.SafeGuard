using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Models;

namespace ZoDream.SafeGuard.DataNet
{
    public class Train
    {

        public static readonly string ModelPath = Path.Combine(Environment.CurrentDirectory, "data", "SdcaLogisticRegression_Model.zip");
        private readonly string InputFolder = Path.Combine(Environment.CurrentDirectory, "data", "input");
        private readonly Encoding Encoding = new UTF8Encoding(false);

        public void AddFile(IEnumerable<string> items, FileCheckStatus status)
        {
            // 判断文件是否存在

        }
        public void AddFile(string fileName, FileCheckStatus status)
        {
            // 判断文件是否存在
            // 生成不重复文件名
            var name = Path.GetFileNameWithoutExtension(fileName);
            var target = Path.Combine(InputFolder, Enum.GetName(status)!, $"{name}.{StorageFinder.GetExtension(fileName)}");
            File.WriteAllText(target, ReadFile(fileName), Encoding);
        }

        public void TrainAndSave()
        {
            var context = new MLContext();
            // var dataPath = Path.Combine(Environment.CurrentDirectory, "data", "input.csv");
            IEnumerable<CheckData> files = LoadDataFromDirectory(InputFolder);
            var fullData = context.Data.LoadFromEnumerable(files);//context.Data.LoadFromTextFile<CheckData>(path: dataPath, hasHeader: true, separatorChar: ',');
            // var trainTestData = context.Data.TrainTestSplit(fullData, testFraction: .2);
            // var trainData = trainTestData.TrainSet;
            BuildAndTrainModel(context, fullData);
        }

        public ITransformer BuildAndTrainModel(MLContext context, IDataView data)
        {
            var dataProcessPipeline = context.Transforms.Conversion.MapValueToKey("Label", "Label")
                                      .Append(context.Transforms.Text.FeaturizeText("FeaturesText", new Microsoft.ML.Transforms.Text.TextFeaturizingEstimator.Options
                                      {
                                          WordFeatureExtractor = new Microsoft.ML.Transforms.Text.WordBagEstimator.Options { NgramLength = 2, UseAllLengths = true },
                                          CharFeatureExtractor = new Microsoft.ML.Transforms.Text.WordBagEstimator.Options { NgramLength = 3, UseAllLengths = false },
                                          Norm = Microsoft.ML.Transforms.Text.TextFeaturizingEstimator.NormFunction.L2,
                                      }, "Text"))
                                      .Append(context.Transforms.Text.FeaturizeText(inputColumnName: "Extension", outputColumnName: "ExtensionFeaturized"))
                                      .Append(context.Transforms.Concatenate("Features", "FeaturesText", "ExtensionFeaturized"))
                                      //.Append(context.Transforms.CopyColumns("Features", "FeaturesText"))
                                      .AppendCacheCheckpoint(context);


            var trainer = context.MulticlassClassification.Trainers.OneVersusAll(context.BinaryClassification.Trainers.AveragedPerceptron(labelColumnName: "Label", numberOfIterations: 10, featureColumnName: "Features"), labelColumnName: "Label")
                                      .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
            var trainingPipeLine = dataProcessPipeline.Append(trainer);

            // 验证
            // var crossValidationResults = context.MulticlassClassification.CrossValidate(data: data, estimator: trainingPipeLine, numberOfFolds: 5);
            // ConsoleHelper.PrintMulticlassClassificationFoldsAverageMetrics(trainer.ToString(), crossValidationResults);
            var model = trainingPipeLine.Fit(data);
            // 生成完成
            context.Model.Save(model, data.Schema, ModelPath);
            return model;
        }

        private IEnumerable<CheckData> LoadDataFromDirectory(string root)
        {
            var rootInfo = new DirectoryInfo(root);
            var items = new List<CheckData>();
            foreach (var folder in rootInfo.EnumerateDirectories())
            {
                foreach (var file in folder.EnumerateFiles())
                {
                    items.Add(new CheckData()
                    {
                        Label = folder.Name,
                        Extension = StorageFinder.GetExtension(file),
                        Text = ReadFile(file),
                    });
                }
            }
            return items;
        }


        private string ReadFile(FileInfo file)
        {
            return ReadFile(file.FullName);
        }

        private string ReadFile(string fileName)
        {
            return File.ReadAllText(fileName, Encoding);
        }
    }
}
