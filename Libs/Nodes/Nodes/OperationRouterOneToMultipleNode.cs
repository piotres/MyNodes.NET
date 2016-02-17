﻿//planer-pro copyright 2015 GPL - license.

using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes
{
    public class OperationRouterOneToMultipleNode : Node
    {
        public OperationRouterOneToMultipleNode() : base("Operation", "Router One-Multiple")
        {
            AddInput("Output Number", DataType.Number);
            AddInput("Value", DataType.Text, true);
            AddOutput();
            AddOutput();

            options.ResetOutputsIfAnyInputIsNull = true;

            Settings.Add("outs", new NodeSetting(NodeSettingType.Number, "Outputs count", Outputs.Count.ToString()));
        }


        public override void OnInputChange(Input input)
        {
            try
            {
                int index = (int)double.Parse(Inputs[0].Value);


                if (index < 1 || index > Outputs.Count)
                {
                    LogError("Output Number is out of range");
                    return;
                }

                Outputs[index - 1].Value = Inputs[1].Value;
            }
            catch
            {
                LogIncorrectInputValueError(Inputs[0]);
            }
        }

        public override bool SetSettings(Dictionary<string, string> data)
        {
            int count = int.Parse(data["outs"]);

            if (count < 2)
                count = 2;

            else if (count > 1000)
                count = 1000;

            data["outs"] = count.ToString();

            if (count > Outputs.Count)
            {
                int addCount = count - Outputs.Count;

                for (int i = 0; i < addCount; i++)
                    AddOutput();

                LogInfo($"Added {addCount} new outputs");
                UpdateMe();
                UpdateMeInDb();
            }
            else if (count < Outputs.Count)
            {
                int remCount = Outputs.Count - count;

                for (int i = 0; i < remCount; i++)
                    RemoveOutput(Outputs.Last());

                LogInfo($"Removed {remCount} outputs");
                UpdateMe();
                UpdateMeInDb();
            }


            return base.SetSettings(data);
        }
    }
}