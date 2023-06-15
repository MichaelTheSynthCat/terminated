using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ObjectivesController checks if are objectives (Gameobjects with ObjectiveScript component attached) are fulfilled.
//  It controls when a level should end.
public class ObjectivesController : MonoBehaviour
{
    public const float end_game_deadline = 3.0f;
    private static float end_game_in = -1.0f; // time until the game will end due to status of objectives, -1.0f if it's not going to
    public Color completed_objective_color; // color for completed objective
    public Color failed_objective_color; // color for failed objective
    public GameObject score_menu; // score panel object

    // informations to be shown when level ends
    private bool victory = false;
    private string detail = "All main objectives completed!"; // detail about the result of the level
    private float completion_time; // time played
    private int completed_objectives = 0;

    public static ObjectivesController instance;
    private void Awake()
    {
        end_game_in = -1.0f;
        instance = this;
    }
    void Update()
    {
        if (GameGoingToEnd()) // game is going to end
        {
            if(end_game_in > 0.0f) end_game_in -= Time.deltaTime;
            if (end_game_in <= 0.0f) EndGame(); // end game, show score menu
        }
        else
        {
            bool fail = false;
            if (DeviceManagment.GetOperator(1).GetComponent<OperatorScript>().WasDefeated())
                // user's operator was defeated, game ends
            {
                fail = true;
                detail = "Lost control to your operator!";
            }
            int completed_main_objectrives = 0;
            completed_objectives = 0;
            foreach (ObjectiveScript objective in ObjectiveScript.objectives)
            {
                // check all active objectives
                if (!objective.gameObject.activeSelf) // don't control the objective if it's not active
                {
                    continue;
                }
                if (!objective.bonus_objective) 
                {
                    // main objectives
                    if (objective.success_objective)
                    {
                        // goals
                        if (objective.Fulfilled())
                        {
                            objective.ChangeColor(completed_objective_color);
                            completed_objectives++;
                            completed_main_objectrives++;
                        }
                    }
                    else
                    {
                        // requirements
                        if (objective.Fulfilled())
                        {
                            completed_main_objectrives++;
                            completed_objectives++;
                        }
                        else
                        {
                            objective.ChangeColor(failed_objective_color);
                            fail = true;
                            detail = objective.GetFailDetail();
                        }
                    }
                }
                else if (objective.bonus_objective)
                {
                    // bonus objectives
                    if (objective.Fulfilled())
                    {
                        completed_objectives++;
                    }
                }
            }
            if (fail) // level failed
            {
                completion_time = TimerController.time;
                end_game_in = end_game_deadline;
            }
            else if(completed_main_objectrives == ObjectiveScript.main_objectives
                && ObjectiveScript.main_objectives != 0) // all main objectvies completed
            {
                completion_time = TimerController.time;
                end_game_in = end_game_deadline;
                victory = true;
            }
        }
    }
    public static bool GameGoingToEnd()
    {
        return end_game_in > -1.0f;
    }
    public void EndGame()
    {
        end_game_in = -1.0f;
        score_menu.SetActive(true);
        PauseMenu.EndGame();
        ScoreMenu.OpenScoreMenuStatic(victory, detail, completion_time);
    }
}
