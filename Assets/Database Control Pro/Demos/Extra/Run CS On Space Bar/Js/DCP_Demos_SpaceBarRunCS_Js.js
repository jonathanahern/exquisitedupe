#pragma strict
import DatabaseControl; //        << Reference to namespace needed


//Variables which are set in the Inspector
var databaseName = "Database Name";
var csName = "Sequence Name";
var inputValues : String[];


function Update () {
    //Check if the Space Bar is pressed
    if (Input.GetKeyDown(KeyCode.Space))
    {
        Debug.Log("Running Sequence ...");

        //Runs function to run sequence
        RunSequence();
    }
}

function RunSequence () { //   << It is easier to keep this code in a seperate function as it includes a delay

    //Run the Command Sequence providing the Database Name, Command Sequence Name and the string[] for the input variable's values
    var e = DCP.RunCS(databaseName, csName, inputValues); // The inputValues must match up with the input variables of the Command Sequence which can be seen on the start node of the sequence in the Sequencer Window. They must be in the same order.

    //You could use:
    //var e = DCP.RunCS(databaseName, csName);
    //if your sequence doesn't have any input variables

    //Wait for the sequence to finish
    while(e.MoveNext()) {
        yield e.Current;
    }
    var returnText = e.Current; //retrieve the result

    //Show the result in the Console
    Debug.Log("Result: " + returnText); 
}