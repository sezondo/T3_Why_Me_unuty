using UnityEngine;

public class RobButtonNet : RobButton
{
    public RobPreviewData robPreview01;
    public RobPreviewData robPreview02;
    public RobPreviewData robPreview03;
    public RobPreviewData robPreview04;
    public RobPreviewData robPreview05;
    public RobPreviewData robPreview06;
    public RobPreviewData robPreview07;
    public RobPreviewData robPreview08;
    public RobPreviewData robPreview12;
    public override void Start()
    {
        //if (Matchmaker.Runner.IsServer || true)
        if (true)
        {
            previewUnitPrefab01 = robPreview01.RobPreviewHost;
            previewUnitPrefab02 = robPreview02.RobPreviewHost;
            previewUnitPrefab03 = robPreview03.RobPreviewHost;
            previewUnitPrefab04 = robPreview04.RobPreviewHost;
            previewUnitPrefab05 = robPreview05.RobPreviewHost;
            previewUnitPrefab06 = robPreview06.RobPreviewHost;
            previewUnitPrefab07 = robPreview07.RobPreviewHost;
            previewUnitPrefab08 = robPreview08.RobPreviewHost;
            previewUnitPrefab012 = robPreview12.RobPreviewHost;
        }
        /*
        if (Matchmaker.Runner.IsClient)
        {
            previewUnitPrefab01 = robPreview01.RobPreviewClient;
            previewUnitPrefab02 = robPreview02.RobPreviewClient;
            previewUnitPrefab03 = robPreview03.RobPreviewClient;
            previewUnitPrefab04 = robPreview04.RobPreviewClient;
            previewUnitPrefab05 = robPreview05.RobPreviewClient;
            previewUnitPrefab06 = robPreview06.RobPreviewClient;
            previewUnitPrefab07 = robPreview07.RobPreviewClient;
            previewUnitPrefab08 = robPreview08.RobPreviewClient;
            previewUnitPrefab012 = robPreview12.RobPreviewClient;
        }
        */
    }
}
