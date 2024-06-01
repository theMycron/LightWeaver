using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class TerminalInterface : MonoBehaviour
{
    [Header("Enter messages in sequence")]
    [Tooltip("r: Robot, p: Protagonist, s: System")]
    [SerializeField] private List<string> messageList;
    private TerminalTextManager terminal;
    private int currentMessageIndex = 0;

    private void Awake()
    {
        terminal = FindAnyObjectByType<TerminalTextManager>();
    }

    public void SetTerminalManager(TerminalTextManager ttm)
    {
        if ( ttm != null )
            terminal = ttm;
    }
    public bool HasTerminalManager()
    {
        return ( terminal != null );
    }

    public void PrintNextMessage()
    {
        if (messageList != null && messageList.Count > 0)
            AddMessageParsed(messageList[currentMessageIndex++]);
    }

    private void AddMessageParsed(string message)
    {
        if (message == null || message.Length < 2)
        {
            return;
        }

        char type = message.ToCharArray()[0];
        message = message.Substring(2);
        switch (type)
        {
            case 's':
                AddSystemMessage(message);
                break;
            case 'r':
                AddRobotMessage(message);
                break;
            case 'p':
                AddProtagonistMessage(message);
                break;
            default:
                return;
        }
    }

    // prints a line entered by the main character into the terminal
    public void AddProtagonistMessage(string message)
    {
        AddPrefixedLine("MSG (SLK-9): ", message);
    }
    // prints a line entered by the system into the terminal
    public void AddSystemMessage(string message)
    {
        AddPrefixedLine("MSG (SYSTEM): ", message);
    }
    // prints a line entered by the robot system into the terminal
    public void AddRobotMessage(string message)
    {
        AddPrefixedLine("MSG (ROBOT): ", message);
    }

    public void AddPrefixedLine(string prefix, string message)
    {
        terminal.AddNewLine(prefix + message);
    }

    public void AddTerminalLine(string text)
    {
        terminal.AddNewLine(text);
    }
}
