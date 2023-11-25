using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace AnimGraphParser
{
    public static class Program
    {
        static string AssetFilePath = @"C:\Users\Oleg\Downloads\ABSub_Survivor00_Locomotion_Injured.uasset";
        public static UAsset Asset = new UAsset(AssetFilePath, EngineVersion.VER_UE4_27);

        public static List<BakedAnimationStateMachine> StateMachines = new List<BakedAnimationStateMachine>();
        public static List<StructPropertyData> AnimNodeProperties = new List<StructPropertyData>();        

        static void Main(string[] Args)
        {
            /** Get Animation Blueprint class export */
            List<NormalExport> AnimBlueprintGeneratedClassExports = FindExportsByClassName("AnimBlueprintGeneratedClass");
            if (AnimBlueprintGeneratedClassExports.Count != 1)
                return;
            ClassExport AnimBlueprintGeneratedClass = (ClassExport)AnimBlueprintGeneratedClassExports[0];

            /** Get all state machines in this class */
            ArrayPropertyData? BakedStateMachinesData = null;
            foreach (PropertyData data in AnimBlueprintGeneratedClass.Data)
                if (data.Name.ToString() == "BakedStateMachines")
                    BakedStateMachinesData = (ArrayPropertyData)data;

            if (BakedStateMachinesData is not null)
                foreach (StructPropertyData StateMachineStruct in BakedStateMachinesData.Value.Cast<StructPropertyData>())
                    StateMachines.Add(new BakedAnimationStateMachine(StateMachineStruct));

            /** Get ClassDefaultObject export */
            NormalExport? ClassDefaultObject = null;
            foreach (Export Export in Asset.Exports)
                if (Export.ClassIndex.IsExport())
                    ClassDefaultObject = (NormalExport)Export;

            if (ClassDefaultObject is null)
                return;

            /** Get all AnimNode properties in this class */
            foreach (PropertyData data in ClassDefaultObject.Data)
            {
                if (data is StructPropertyData Property && Property.Name.ToString().StartsWith("AnimGraphNode_"))
                    AnimNodeProperties.Add(Property);
            }

            /** Get Root Node */
            AnimNode_Base? Root = null;
            foreach (StructPropertyData AnimNodeProperty in AnimNodeProperties)
                if (AnimNodeProperty.StructType.ToString() == "AnimNode_Root")
                {
                    Root = AnimNode_Base.ConvertStructToAnimNode(AnimNodeProperty);
                    break;
                }

            if (Root is null)
            {
                /** This AnimBlueprint is child of another AnimBlueprint */
                Console.WriteLine("This Animation Blueprint is derived from another blueprint");

                string SuperStruct = AnimBlueprintGeneratedClass.SuperStruct.ToImport(Asset).ObjectName.ToString();
                Console.WriteLine($"Analyze {SuperStruct} instead");
                
                return;
            }

            PrintNodeInfo(new KeyValuePair<string, AnimNode_Base>("", Root));
            Console.WriteLine("\r\n\r\n");

            /** Get all SaveCachedPose nodes */
            List<AnimNode_Base> SaveCachedPose_Nodes = new List<AnimNode_Base>();

            foreach (StructPropertyData AnimNodeProperty in AnimNodeProperties)
            {
                if (AnimNodeProperty.StructType.ToString() == "AnimNode_SaveCachedPose")
                    SaveCachedPose_Nodes.Add(AnimNode_Base.ConvertStructToAnimNode(AnimNodeProperty));
            }

            foreach (AnimNode_Base SaveCachedPose_Node in SaveCachedPose_Nodes)
            {
                PrintNodeInfo(new KeyValuePair<string, AnimNode_Base>("", SaveCachedPose_Node));
                Console.WriteLine("\r\n\r\n");
            }


            /** Print out all state machines */
            Console.WriteLine("");
            foreach (BakedAnimationStateMachine StateMachine in StateMachines)
            {
                Console.WriteLine("***************************\r\n***");
                Console.WriteLine($"***   State Machine:");
                Console.WriteLine($"***   {StateMachine.MachineName}");
                Console.WriteLine("***\r\n***************************\r\n");

                foreach (BakedAnimationState State in StateMachine.States)
                {
                    if (State.StateRootNodeIndex == -1)
                        continue;

                    AnimNode_Base StateResult =
                        AnimNode_Base.ConvertStructToAnimNode(AnimNodeProperties[AnimNodeProperties.Count - 1 - State.StateRootNodeIndex]);

                    Console.WriteLine($"State: {State.StateName}");
                    Console.WriteLine();

                    PrintNodeInfo(new KeyValuePair<string, AnimNode_Base>("", StateResult));
                    Console.WriteLine("\r\n\r\n");
                }

                Console.WriteLine("\r\n\r\n\r\n\r\n");
            }
        }

        static void PrintNodeInfo(KeyValuePair<string, AnimNode_Base> AnimNodePair, int NestingLevel = -1)
        {
            string InputPinName = AnimNodePair.Key;
            AnimNode_Base AnimNode = AnimNodePair.Value;

            if (NestingLevel != -1)
            {
                for (int i = 0; i < NestingLevel; i++)
                    Console.Write('\t');

                Console.WriteLine($"- {InputPinName}:");
            }

            for (int i = 0; i < NestingLevel + 1; i++)
                Console.Write('\t');

            Console.WriteLine(AnimNode.GetNodeInfo());

            foreach (KeyValuePair<string, AnimNode_Base> InputNode in AnimNode.InputNodes)
                PrintNodeInfo(InputNode, NestingLevel + 1);

            //if (NestingLevel == -1)
            //    Console.WriteLine("\r\n\r\n");
        }

        static string GetTargetStateNameForTransitionIndex(ArrayPropertyData States, ArrayPropertyData Transitions, int Index)
        {
            StructPropertyData AnimationTransitionBetweenStates = (StructPropertyData)Transitions.Value[Index];

            IntPropertyData? NextState = FindPropertyInStruct(AnimationTransitionBetweenStates, "NextState") as IntPropertyData;
            if (NextState == null)
                return "";

            int TargetStateIndex = NextState.Value;

            StructPropertyData BakedAnimationState = (StructPropertyData)States.Value[TargetStateIndex];
            NamePropertyData? StateName = FindPropertyInStruct(BakedAnimationState, "StateName") as NamePropertyData;

            if (StateName is not null)
                return StateName.Value.ToString();
            return "";
        }

        public static PropertyData? FindPropertyInStruct(StructPropertyData Struct, string PropertyName)
        {
            foreach (PropertyData data in Struct.Value)
                if (data.Name.ToString() == PropertyName)
                    return data;

            return null;
        }

        public static T FindPropertyInStruct<T>(StructPropertyData Struct, string PropertyName) where T : PropertyData
        {
            foreach (PropertyData data in Struct.Value)
                if (data.Name.ToString() == PropertyName)
                    return (T)data;

            throw new Exception($"Failed to find property {PropertyName} in {Struct.Name}");
        }

        static List<NormalExport> FindExportsByClassName(string ClassName)
        {
            if (Asset is null)
                throw new Exception("Asset was null when tried to find export by class name");

            List<NormalExport> OutExports = new List<NormalExport>();

            for (int i = 0; i < Asset.Exports.Count; i++)
            {
                Export export = Asset.Exports[i];

                if (!(export is NormalExport normalExport))
                    continue;

                if (!export.ClassIndex.IsImport())
                    continue;

                Import Class = export.ClassIndex.ToImport(Asset);
                if (Class.ObjectName.ToString() == ClassName)
                    OutExports.Add(normalExport);
            }

            return OutExports;
        }
    }
}
