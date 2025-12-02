using UnityEngine;

public class CounterList : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private UIAppearance uiAppearance;
    
    [Header("Dialog Data for Each Count")]
    [SerializeField] private DialogData dialogData1;
    [SerializeField] private DialogData dialogData2;
    [SerializeField] private DialogData dialogData3;
    [SerializeField] private DialogData dialogData4;
    
    private int counter = 0;
    
    public void IncrementCounter()
    {
        counter++;
        
        DialogData selectedDialog = null;
        
        switch (counter)
        {
            case 1:
                selectedDialog = dialogData1;
                break;
            case 2:
                selectedDialog = dialogData2;
                break;
            case 3:
                selectedDialog = dialogData3;
                break;
            case 4:
                selectedDialog = dialogData4;
                break;
        }
        
        if (selectedDialog != null && uiAppearance != null)
        {
            uiAppearance.SetDialogData(selectedDialog);
            uiAppearance.StartDialog();
        }
    }
    
    public int GetCounter()
    {
        return counter;
    }
    
    public void ResetCounter()
    {
        counter = 0;
    }
}
