using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace dnaborshchikova_github.Bea.Collector.Tests.Processor.Builders
{
    public class ProcessingSettingsBuilder
    {
        private ProcessingSettings _processingSettings;

        public static ProcessingSettingsBuilder Default()
        {
            return new ProcessingSettingsBuilder
            {
                _processingSettings = new ProcessingSettings
                {
                    GeneratorRunAsProcess = false,
                    GenerateFile = false,
                    FilePath = "C:\\BillEvent.csv",
                    ThreadCount = 1,
                    ProcessType = "Thread"
                }
            };
        }

        public ProcessingSettingsBuilder WithThreadCount(int count)
        {
            _processingSettings.ThreadCount = count;
            return this;
        }

        public ProcessingSettingsBuilder WithProcessType(string type)
        {
            _processingSettings.ProcessType = type;
            return this;
        }

        public ProcessingSettingsBuilder WithFilePath(string path)
        {
            _processingSettings.FilePath = path;
            return this;
        }

        public ProcessingSettings Build()
        {
            return _processingSettings;
        }
    }
}
