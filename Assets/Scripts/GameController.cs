using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    public GUIText playersTurnText, movesText, gameOverText;            //text shown to indicate players turn, how many moves left and the winner of the game
    string player;                                                      //used to show who's turn it is
    int count;

    /*
     * creates the board map
     * 
     * line - a horizontal line between the points
     * vrline - vertical line slanted on the right side -> /
     * drline - diagonal line slanted to the left side -> \
     * hbridge - horizontal bridge
     * vbridge - vertical bridge
     * dbridge - diagonal bridge
     * 
    */
    public int xSize, zSize;
    public GameObject point, line, sdline, vrline, drline, hbridge, vbridge, dbridge;
    public GameObject redCities, greenCities, yellowCities, blueCities, orangeCities;


    /*
     * players victory points (victory points are basically the cities to connect)
    */
    public GameObject p1vp, p2vp, p3vp;


    public int humanPlayers;        //indicates how many human players are playing the game
    public int botPlayers;          //indicates how many bot players are playing the game

    private Vector3[] vertices;
    private Vector3[] horizontalLines, verticalLines, diagonalLines;
    //	private Vector3[] players;

    /*
     * Stores the value of the users hubs which is given to them right now.
     * In order to have the user choose the color of their hub, we need to program that when 
     * we work on the UI.
     * 
     * TODO: Assign the color of choice to the user's hub
    */
    public GameObject userHubs1, userHubs2, userHubs3, userHubs4, userHubs5, userHubs6;

    private int maxHubs;            //holds the total number hubs to be placed (i.e. human + bot players)
    private int hubCount;           //indicates the number of hubs placed

    ArrayList definedPoints = new ArrayList();

    /*
     * There are 5 zones on the board, each zone indicates a number of cities.
     * zone1 - green cities
     * zone2 - blue cities
     * zone3 - red cities
     * zone4 - orange cities
     * zone5 - yellow cities
     * 
     * Each zone has 7 cities where 2 of the cities from each zone are exclusive
     * if there are <= 5 players playing the game, but included when there are 6 players playing.
    */
    ArrayList zone1 = new ArrayList();
    ArrayList zone2 = new ArrayList();
    ArrayList zone3 = new ArrayList();
    ArrayList zone4 = new ArrayList();
    ArrayList zone5 = new ArrayList();
    ArrayList zone1_full = new ArrayList();
    ArrayList zone2_full = new ArrayList();
    ArrayList zone3_full = new ArrayList();
    ArrayList zone4_full = new ArrayList();
    ArrayList zone5_full = new ArrayList();

    ArrayList pointCollections = new ArrayList();

    //	ArrayList quadrant1 = new ArrayList ();
    //	ArrayList quadrant2 = new ArrayList ();
    //	ArrayList quadrant3 = new ArrayList ();
    //	ArrayList quadrant4 = new ArrayList ();

    ArrayList playersHubCollections;

    ArrayList[] players = new ArrayList[6];
    ArrayList[] playersCollections = new ArrayList[6];

    ArrayList botsvp = new ArrayList();

    public ArrayList bridgeNodes = new ArrayList();

    Graph graph;

    public GameObject TrackBlocks;

    Ray ray;
    RaycastHit hit;

    bool gotpaths;

    int turn;
    int layedTracks;

    int allow_bot_to_lay_tracks;

    private bool restart;               //is used to restart the game


    // HOLDS THE POSITION OF THE CITIES ON THE MAP
    Dictionary<Vector3, string> city_zones = new Dictionary<Vector3, string>() {
		//green
		{new Vector3(6f, 1f, 2f), "zone1_full"},
        {new Vector3(5f, 1f, 3f), "zone1"},
        {new Vector3(2f, 1f, 6f), "zone1"},
        {new Vector3(2f, 1f, 7f), "zone1"},
        {new Vector3(1f, 1f, 9f), "zone1"},
        {new Vector3(0f, 1f, 11f), "zone1"},
        {new Vector3(0f, 1f, 12f), "zone1_full"},
		//blue
		{new Vector3(3f, 1f, 11f), "zone2"},
        {new Vector3(7f, 1f, 11f), "zone2"},
        {new Vector3(10f, 1f, 11f), "zone2_full"},
        {new Vector3(10f, 1f, 10f), "zone2"},
        {new Vector3(13f, 1f, 9f), "zone2"},
        {new Vector3(15f, 1f, 10f), "zone2_full"},
        {new Vector3(15f, 1f, 7f), "zone2"},
		//red
		{new Vector3(7f, 1f, 3f), "zone3"},
        {new Vector3(10f, 1f, 1f), "zone3"},
        {new Vector3(13f, 1f, 2f), "zone3"},
        {new Vector3(14f, 1f, 0f), "zone3_full"},
        {new Vector3(16f, 1f, 0f), "zone3"},
        {new Vector3(15f, 1f, 3f), "zone3"},
        {new Vector3(17f, 1f, 2f), "zone3_full"},
		//orange
		{new Vector3(17f, 1f, 10f), "zone4_full"},
        {new Vector3(17f, 1f, 7f), "zone4"},
        {new Vector3(17f, 1f, 8f), "zone4"},
        {new Vector3(18f, 1f, 5f), "zone4_full"},
        {new Vector3(17f, 1f, 4f), "zone4"},
        {new Vector3(19f, 1f, 2f), "zone4"},
        {new Vector3(19f, 1f, 0f), "zone4"},
		//yellow
		{new Vector3(4f, 1f, 8f), "zone5"},
        {new Vector3(7f, 1f, 7f), "zone5_full"},
        {new Vector3(8f, 1f, 4f), "zone5"},
        {new Vector3(9f, 1f, 8f), "zone5"},
        {new Vector3(11f, 1f, 6f), "zone5_full"},
        {new Vector3(11f, 1f, 4f), "zone5"},
        {new Vector3(13f, 1f, 6f), "zone5"},
    };

    // HOLDS THE POSITION OF THE HORIZONTAL BRIDGES ON THE MAP
    Dictionary<Vector3, string> horizontal_bridges = new Dictionary<Vector3, string>() {
        { new Vector3 (0f, 1f, 12f), "hbridge" },
        { new Vector3 (0f, 1f, 11f), "hbridge" },
        { new Vector3 (0f, 1f, 10f), "hbridge" },
        { new Vector3 (1f, 1f, 9f), "hbridge" },
        { new Vector3 (1f, 1f, 8f), "hbridge" },
        { new Vector3 (2f, 1f, 7f), "hbridge" },
        { new Vector3 (3f, 1f, 7f), "hbridge" },
        { new Vector3 (3f, 1f, 6f), "hbridge" },
        { new Vector3 (3f, 1f, 5f), "hbridge" },
        { new Vector3 (4f, 1f, 4f), "hbridge" },
        { new Vector3 (5f, 1f, 4f), "hbridge" },
        { new Vector3 (6f, 1f, 4f), "hbridge" },
        { new Vector3 (5f, 1f, 3f), "hbridge" },
        { new Vector3 (6f, 1f, 3f), "hbridge" },
        { new Vector3 (7f, 1f, 3f), "hbridge" },
        { new Vector3 (6f, 1f, 2f), "hbridge" },
        { new Vector3 (5f, 1f, 5f), "hbridge" },
        { new Vector3 (5f, 1f, 6f), "hbridge" },
        { new Vector3 (7f, 1f, 6f), "hbridge" },
        { new Vector3 (7f, 1f, 5f), "hbridge" },
        { new Vector3 (6f, 1f, 7f), "hbridge" },
        { new Vector3 (5f, 1f, 9f), "hbridge" },
        { new Vector3 (4f, 1f, 10f), "hbridge" },
        { new Vector3 (3f, 1f, 10f), "hbridge" },
        { new Vector3 (3f, 1f, 11f), "hbridge" },
        { new Vector3 (2f, 1f, 11f), "hbridge" },
        { new Vector3 (6f, 1f, 11f), "hbridge" },
        { new Vector3 (7f, 1f, 10f), "hbridge" },
        { new Vector3 (8f, 1f, 9f), "hbridge" },
        { new Vector3 (9f, 1f, 8f), "hbridge" },
        { new Vector3 (10f, 1f, 7f), "hbridge" },
        { new Vector3 (12f, 1f, 7f), "hbridge" },
        { new Vector3 (11f, 1f, 8f), "hbridge" },
        { new Vector3 (11f, 1f, 9f), "hbridge" },
        { new Vector3 (10f, 1f, 10f), "hbridge" },
        { new Vector3 (9f, 1f, 11f), "hbridge" },
        { new Vector3 (13f, 1f, 6f), "hbridge" },
        { new Vector3 (14f, 1f, 6f), "hbridge" },
        { new Vector3 (14f, 1f, 5f), "hbridge" },
        { new Vector3 (14f, 1f, 4f), "hbridge" },
        { new Vector3 (14f, 1f, 3f), "hbridge" },
        { new Vector3 (14f, 1f, 2f), "hbridge" },
        { new Vector3 (15f, 1f, 1f), "hbridge" },
        { new Vector3 (15f, 1f, 0f), "hbridge" },
        { new Vector3 (16f, 1f, 3f), "hbridge" },
        { new Vector3 (16f, 1f, 4f), "hbridge" },
        { new Vector3 (16f, 1f, 5f), "hbridge" },
        { new Vector3 (16f, 1f, 6f), "hbridge" },
        { new Vector3 (16f, 1f, 7f), "hbridge" },
        { new Vector3 (16f, 1f, 8f), "hbridge" }
    };

    // HOLDS THE POSITION OF THE VERTICAL BRIDGES ON THE MAP
    Dictionary<Vector3, string> vertical_bridges = new Dictionary<Vector3, string>() {
        { new Vector3 (2f, 1f, 11f), "vbridge" },
        { new Vector3 (4f, 1f, 11f), "vbridge" },
        { new Vector3 (5f, 1f, 11f), "vbridge" },
        { new Vector3 (6f, 1f, 11f), "vbridge" },
        { new Vector3 (3f, 1f, 10f), "vbridge" },
        { new Vector3 (4f, 1f, 10f), "vbridge" },
        { new Vector3 (7f, 1f, 10f), "vbridge" },
        { new Vector3 (1f, 1f, 9f), "vbridge" },
        { new Vector3 (5f, 1f, 9f), "vbridge" },
        { new Vector3 (8f, 1f, 9f), "vbridge" },
        { new Vector3 (9f, 1f, 8f), "vbridge" },
        { new Vector3 (2f, 1f, 7f), "vbridge" },
        { new Vector3 (3f, 1f, 7f), "vbridge" },
        { new Vector3 (5f, 1f, 7f), "vbridge" },
        { new Vector3 (10f, 1f, 7f), "vbridge" },
        { new Vector3 (12f, 1f, 7f), "vbridge" },
        { new Vector3 (6f, 1f, 6f), "vbridge" },
        { new Vector3 (7f, 1f, 6f), "vbridge" },
        { new Vector3 (11f, 1f, 6f), "vbridge" },
        { new Vector3 (12f, 1f, 6f), "vbridge" },
        { new Vector3 (13f, 1f, 6f), "vbridge" },
        { new Vector3 (15f, 1f, 6f), "vbridge" },
        { new Vector3 (3f, 1f, 5f), "vbridge" },
        { new Vector3 (14f, 1f, 5f), "vbridge" },
        { new Vector3 (6f, 1f, 3f), "vbridge" },
        { new Vector3 (7f, 1f, 3f), "vbridge" },
        { new Vector3 (8f, 1f, 2f), "vbridge" },
        { new Vector3 (15f, 1f, 1f), "vbridge" },
        { new Vector3 (10f, 1f, 10f), "vbridge" },
        { new Vector3 (11f, 1f, 9f), "vbridge" }
    };

    // INDICATES THE POSITION OF THE DIAGONAL BRIDGES ON THE MAP
    Dictionary<Vector3, string> diagonal_bridges = new Dictionary<Vector3, string>() {
        {new Vector3(1f, 1f, 11f), "dbridge"},
        {new Vector3(5f, 1f, 11f), "dbridge"},
        {new Vector3(6f, 1f, 11f), "dbridge"},
        {new Vector3(1f, 1f, 10f), "dbridge"},
        {new Vector3(2f, 1f, 8f), "dbridge"},
        {new Vector3(12f, 1f, 8f), "dbridge"},
        {new Vector3(17f, 1f, 8f), "dbridge"},
        {new Vector3(17f, 1f, 7f), "dbridge"},
        {new Vector3(3f, 1f, 6f), "dbridge"},
        {new Vector3(4f, 1f, 6f), "dbridge"},
        {new Vector3(6f, 1f, 6f), "dbridge"},
        {new Vector3(12f, 1f, 6f), "dbridge"},
        {new Vector3(13f, 1f, 6f), "dbridge"},
        {new Vector3(15f, 1f, 6f), "dbridge"},
        {new Vector3(16f, 1f, 6f), "dbridge"},
        {new Vector3(17f, 1f, 6f), "dbridge"},
        {new Vector3(6f, 1f, 5f), "dbridge"},
        {new Vector3(8f, 1f, 5f), "dbridge"},
        {new Vector3(15f, 1f, 5f), "dbridge"},
        {new Vector3(17f, 1f, 5f), "dbridge"},
        {new Vector3(6f, 1f, 4f), "dbridge"},
        {new Vector3(15f, 1f, 4f), "dbridge"},
        {new Vector3(17f, 1f, 4f), "dbridge"},
        {new Vector3(15f, 1f, 3f), "dbridge"},
        {new Vector3(17f, 1f, 3f), "dbridge"},
        {new Vector3(7f, 1f, 2f), "dbridge"},
        {new Vector3(9f, 1f, 2f), "dbridge"},
        {new Vector3(15f, 1f, 2f), "dbridge"},
        {new Vector3(16f, 1f, 0f), "dbridge"},
    };


    // list of positions of the vertices to remove from the board to create the perfect map
    List<Vector3> delete_vertices = new List<Vector3> {
        new Vector3(0f, 1f, 0f),
        new Vector3(1f, 1f, 0f),
        new Vector3(2f, 1f, 0f),
        new Vector3(3f, 1f, 0f),
        new Vector3(4f, 1f, 0f),
        new Vector3(5f, 1f, 0f),
        new Vector3(6f, 1f, 0f),
        new Vector3(7f, 1f, 0f),
        new Vector3(8f, 1f, 0f),
        new Vector3(9f, 1f, 0f),
        new Vector3(10f, 1f, 0f),
        new Vector3(0f, 1f, 1f),
        new Vector3(1f, 1f, 1f),
        new Vector3(2f, 1f, 1f),
        new Vector3(3f, 1f, 1f),
        new Vector3(4f, 1f, 1f),
        new Vector3(5f, 1f, 1f),
        new Vector3(6f, 1f, 1f),
        new Vector3(7f, 1f, 1f),
        new Vector3(0f, 1f, 2f),
        new Vector3(1f, 1f, 2f),
        new Vector3(2f, 1f, 2f),
        new Vector3(3f, 1f, 2f),
        new Vector3(4f, 1f, 2f),
        new Vector3(5f, 1f, 2f),
        new Vector3(0f, 1f, 3f),
        new Vector3(1f, 1f, 3f),
        new Vector3(2f, 1f, 3f),
        new Vector3(3f, 1f, 3f),
        new Vector3(4f, 1f, 3f),
        new Vector3(0f, 1f, 4f),
        new Vector3(1f, 1f, 4f),
        new Vector3(2f, 1f, 4f),
        new Vector3(3f, 1f, 4f),
        new Vector3(0f, 1f, 5f),
        new Vector3(1f, 1f, 5f),
        new Vector3(2f, 1f, 5f),
        new Vector3(19f, 1f, 5f),
        new Vector3(0f, 1f, 6f),
        new Vector3(1f, 1f, 6f),
        new Vector3(18f, 1f, 6f),
        new Vector3(19f, 1f, 6f),
        new Vector3(0f, 1f, 7f),
        new Vector3(1f, 1f, 7f),
        new Vector3(18f, 1f, 7f),
        new Vector3(19f, 1f, 7f),
        new Vector3(0f, 1f, 8f),
        new Vector3(18f, 1f, 8f),
        new Vector3(19f, 1f, 8f),
        new Vector3(0f, 1f, 9f),
        new Vector3(18f, 1f, 9f),
        new Vector3(19f, 1f, 9f),
        new Vector3(13f, 1f, 10f),
        new Vector3(14f, 1f, 10f),
        new Vector3(18f, 1f, 10f),
        new Vector3(19f, 1f, 10f),
        new Vector3(12f, 1f, 11f),
        new Vector3(13f, 1f, 11f),
        new Vector3(14f, 1f, 11f),
        new Vector3(17f, 1f, 11f),
        new Vector3(18f, 1f, 11f),
        new Vector3(19f, 1f, 11f),
        new Vector3(10f, 1f, 12f),
        new Vector3(11f, 1f, 12f),
        new Vector3(12f, 1f, 12f),
        new Vector3(13f, 1f, 12f),
        new Vector3(14f, 1f, 12f),
        new Vector3(15f, 1f, 12f),
        new Vector3(16f, 1f, 12f),
        new Vector3(17f, 1f, 12f),
        new Vector3(18f, 1f, 12f),
        new Vector3(19f, 1f, 12f)
    };

    // list of positions of the horizontal lines to remove from the board to create the perfect map
    List<Vector3> delete_horizontalLines = new List<Vector3> {
        new Vector3(0f, 1f, 0f),
        new Vector3(1f, 1f, 0f),
        new Vector3(2f, 1f, 0f),
        new Vector3(3f, 1f, 0f),
        new Vector3(4f, 1f, 0f),
        new Vector3(5f, 1f, 0f),
        new Vector3(6f, 1f, 0f),
        new Vector3(7f, 1f, 0f),
        new Vector3(8f, 1f, 0f),
        new Vector3(9f, 1f, 0f),
        new Vector3(10f, 1f, 0f),
        new Vector3(0f, 1f, 1f),
        new Vector3(1f, 1f, 1f),
        new Vector3(2f, 1f, 1f),
        new Vector3(3f, 1f, 1f),
        new Vector3(4f, 1f, 1f),
        new Vector3(5f, 1f, 1f),
        new Vector3(6f, 1f, 1f),
        new Vector3(7f, 1f, 1f),
        new Vector3(0f, 1f, 2f),
        new Vector3(1f, 1f, 2f),
        new Vector3(2f, 1f, 2f),
        new Vector3(3f, 1f, 2f),
        new Vector3(4f, 1f, 2f),
        new Vector3(5f, 1f, 2f),
        new Vector3(0f, 1f, 3f),
        new Vector3(1f, 1f, 3f),
        new Vector3(2f, 1f, 3f),
        new Vector3(3f, 1f, 3f),
        new Vector3(4f, 1f, 3f),
        new Vector3(0f, 1f, 4f),
        new Vector3(1f, 1f, 4f),
        new Vector3(2f, 1f, 4f),
        new Vector3(3f, 1f, 4f),
        new Vector3(0f, 1f, 5f),
        new Vector3(1f, 1f, 5f),
        new Vector3(2f, 1f, 5f),
        new Vector3(18f, 1f, 5f),
        new Vector3(0f, 1f, 6f),
        new Vector3(1f, 1f, 6f),
        new Vector3(17f, 1f, 6f),
        new Vector3(18f, 1f, 6f),
        new Vector3(0f, 1f, 7f),
        new Vector3(1f, 1f, 7f),
        new Vector3(17f, 1f, 7f),
        new Vector3(18f, 1f, 7f),
        new Vector3(0f, 1f, 8f),
        new Vector3(17f, 1f, 8f),
        new Vector3(18f, 1f, 8f),
        new Vector3(0f, 1f, 9f),
        new Vector3(17f, 1f, 9f),
        new Vector3(18f, 1f, 9f),
        new Vector3(12f, 1f, 10f),
        new Vector3(13f, 1f, 10f),
        new Vector3(14f, 1f, 10f),
        new Vector3(17f, 1f, 10f),
        new Vector3(18f, 1f, 10f),
        new Vector3(11f, 1f, 11f),
        new Vector3(12f, 1f, 11f),
        new Vector3(13f, 1f, 11f),
        new Vector3(14f, 1f, 11f),
        new Vector3(16f, 1f, 11f),
        new Vector3(17f, 1f, 11f),
        new Vector3(18f, 1f, 11f),
        new Vector3(9f, 1f, 12f),
        new Vector3(10f, 1f, 12f),
        new Vector3(11f, 1f, 12f),
        new Vector3(12f, 1f, 12f),
        new Vector3(13f, 1f, 12f),
        new Vector3(14f, 1f, 12f),
        new Vector3(15f, 1f, 12f),
        new Vector3(16f, 1f, 12f),
        new Vector3(17f, 1f, 12f),
        new Vector3(18f, 1f, 12f)
    };

    // list of positions of the vertical lines to remove from the board to create the perfect map
    List<Vector3> delete_verticalLines = new List<Vector3> {
        new Vector3(0f, 1f, 0f),
        new Vector3(1f, 1f, 0f),
        new Vector3(2f, 1f, 0f),
        new Vector3(3f, 1f, 0f),
        new Vector3(4f, 1f, 0f),
        new Vector3(5f, 1f, 0f),
        new Vector3(6f, 1f, 0f),
        new Vector3(7f, 1f, 0f),
        new Vector3(8f, 1f, 0f),
        new Vector3(9f, 1f, 0f),
        new Vector3(10f, 1f, 0f),
        new Vector3(0f, 1f, 1f),
        new Vector3(1f, 1f, 1f),
        new Vector3(2f, 1f, 1f),
        new Vector3(3f, 1f, 1f),
        new Vector3(4f, 1f, 1f),
        new Vector3(5f, 1f, 1f),
        new Vector3(6f, 1f, 1f),
        new Vector3(7f, 1f, 1f),
        new Vector3(0f, 1f, 2f),
        new Vector3(1f, 1f, 2f),
        new Vector3(2f, 1f, 2f),
        new Vector3(3f, 1f, 2f),
        new Vector3(4f, 1f, 2f),
        new Vector3(5f, 1f, 2f),
        new Vector3(0f, 1f, 3f),
        new Vector3(1f, 1f, 3f),
        new Vector3(2f, 1f, 3f),
        new Vector3(3f, 1f, 3f),
        new Vector3(4f, 1f, 3f),
        new Vector3(0f, 1f, 4f),
        new Vector3(1f, 1f, 4f),
        new Vector3(2f, 1f, 4f),
        new Vector3(3f, 1f, 4f),
        new Vector3(19f, 1f, 4f),
        new Vector3(0f, 1f, 5f),
        new Vector3(1f, 1f, 5f),
        new Vector3(2f, 1f, 5f),
        new Vector3(18f, 1f, 5f),
        new Vector3(19f, 1f, 5f),
        new Vector3(0f, 1f, 6f),
        new Vector3(1f, 1f, 6f),
        new Vector3(18f, 1f, 6f),
        new Vector3(19f, 1f, 6f),
        new Vector3(0f, 1f, 7f),
        new Vector3(1f, 1f, 7f),
        new Vector3(18f, 1f, 7f),
        new Vector3(19f, 1f, 7f),
        new Vector3(0f, 1f, 8f),
        new Vector3(18f, 1f, 8f),
        new Vector3(19f, 1f, 8f),
        new Vector3(0f, 1f, 9f),
        new Vector3(13f, 1f, 9f),
        new Vector3(14f, 1f, 9f),
        new Vector3(18f, 1f, 9f),
        new Vector3(19f, 1f, 9f),
        new Vector3(12f, 1f, 10f),
        new Vector3(13f, 1f, 10f),
        new Vector3(14f, 1f, 10f),
        new Vector3(17f, 1f, 10f),
        new Vector3(18f, 1f, 10f),
        new Vector3(19f, 1f, 10f),
        new Vector3(10f, 1f, 11f),
        new Vector3(11f, 1f, 11f),
        new Vector3(12f, 1f, 11f),
        new Vector3(13f, 1f, 11f),
        new Vector3(14f, 1f, 11f),
        new Vector3(15f, 1f, 11f),
        new Vector3(16f, 1f, 11f),
        new Vector3(17f, 1f, 11f),
        new Vector3(18f, 1f, 11f),
        new Vector3(19f, 1f, 11f)
    };

    // list of positions of the diagonal lines to remove from the board to create the perfect map
    List<Vector3> delete_diagonalLines = new List<Vector3> {
        new Vector3(1f, 1f, 0f),
        new Vector3(2f, 1f, 0f),
        new Vector3(3f, 1f, 0f),
        new Vector3(4f, 1f, 0f),
        new Vector3(5f, 1f, 0f),
        new Vector3(6f, 1f, 0f),
        new Vector3(7f, 1f, 0f),
        new Vector3(8f, 1f, 0f),
        new Vector3(9f, 1f, 0f),
        new Vector3(10f, 1f, 0f),
        new Vector3(1f, 1f, 1f),
        new Vector3(2f, 1f, 1f),
        new Vector3(3f, 1f, 1f),
        new Vector3(4f, 1f, 1f),
        new Vector3(5f, 1f, 1f),
        new Vector3(6f, 1f, 1f),
        new Vector3(7f, 1f, 1f),
        new Vector3(0f, 1f, 2f),
        new Vector3(1f, 1f, 2f),
        new Vector3(2f, 1f, 2f),
        new Vector3(3f, 1f, 2f),
        new Vector3(4f, 1f, 2f),
        new Vector3(5f, 1f, 2f),
        new Vector3(0f, 1f, 3f),
        new Vector3(1f, 1f, 3f),
        new Vector3(2f, 1f, 3f),
        new Vector3(3f, 1f, 3f),
        new Vector3(4f, 1f, 3f),
        new Vector3(0f, 1f, 4f),
        new Vector3(1f, 1f, 4f),
        new Vector3(2f, 1f, 4f),
        new Vector3(3f, 1f, 4f),
        new Vector3(0f, 1f, 5f),
        new Vector3(1f, 1f, 5f),
        new Vector3(2f, 1f, 5f),
        new Vector3(19f, 1f, 5f),
        new Vector3(0f, 1f, 6f),
        new Vector3(1f, 1f, 6f),
        new Vector3(2f, 1f, 6f),
        new Vector3(18f, 1f, 6f),
        new Vector3(19f, 1f, 6f),
        new Vector3(0f, 1f, 7f),
        new Vector3(1f, 1f, 7f),
        new Vector3(18f, 1f, 7f),
        new Vector3(19f, 1f, 7f),
        new Vector3(0f, 1f, 8f),
        new Vector3(1f, 1f, 8f),
        new Vector3(18f, 1f, 8f),
        new Vector3(19f, 1f, 8f),
        new Vector3(0f, 1f, 9f),
        new Vector3(14f, 1f, 9f),
        new Vector3(15f, 1f, 9f),
        new Vector3(18f, 1f, 9f),
        new Vector3(19f, 1f, 9f),
        new Vector3(13f, 1f, 10f),
        new Vector3(14f, 1f, 10f),
        new Vector3(15f, 1f, 10f),
        new Vector3(18f, 1f, 10f),
        new Vector3(19f, 1f, 10f),
        new Vector3(11f, 1f, 11f),
        new Vector3(12f, 1f, 11f),
        new Vector3(13f, 1f, 11f),
        new Vector3(14f, 1f, 11f),
        new Vector3(15f, 1f, 11f),
        new Vector3(16f, 1f, 11f),
        new Vector3(17f, 1f, 11f),
        new Vector3(18f, 1f, 11f),
        new Vector3(19f, 1f, 11f)
    };


    void Awake() {
        maxHubs = humanPlayers + botPlayers;        //sets the value of the var maxHubs
        GenerateRhombicalGrid2();                  //generates a map on the board
        hubCount = 0;
        turn = 0;

        // initializing neccessary arraylists for all players
        for (int i = 0; i < maxHubs; i++)
        {
            players[i] = new ArrayList();
            playersCollections[i] = new ArrayList();
        }

        if (maxHubs > 1)
        {
            playersHubCollections = new ArrayList();   //array to store human player's hubs locations to exclude from bot's hub placement options
            allow_bot_to_lay_tracks = 2;                //counter to track bot's tracks
        }
    }


    void Start() {
        restart = false;
        gotpaths = false;
        gameOverText.text = "";
        UpdateCount();
        UpdateTurn();

        layedTracks = 0;

        // Assigning victory points to each player randomly (one city from each zone)
        for (int i = 0; i < maxHubs; i++)
        {
            int one = Random.Range(0, zone1.Count - 1);
            int two = Random.Range(0, zone2.Count - 1);
            int three = Random.Range(0, zone3.Count - 1);
            int four = Random.Range(0, zone4.Count - 1);
            int five = Random.Range(0, zone5.Count - 1);

            players[i].Add(zone1[one]);
            players[i].Add(zone2[two]);
            players[i].Add(zone3[three]);
            players[i].Add(zone4[four]);
            players[i].Add(zone5[five]);

            zone1.RemoveAt(one);
            zone2.RemoveAt(two);
            zone3.RemoveAt(three);
            zone4.RemoveAt(four);
            zone5.RemoveAt(five);
        }

        //show cities of the first human player with a square above the city
        foreach (Vector3 x in players[0])
        {
            GameObject p1 = Instantiate(p1vp, x + new Vector3(0f, -0.3f, 0.3f), Quaternion.identity) as GameObject;
        }

        //printing all players cities to be connected
        for (int i = 0; i < maxHubs; i++)
        {
            Debug.Log("players size: " + players[i].Count);
            foreach (Vector3 x in players[i])
            {
                Debug.Log("players points: " + x.ToString("F4"));
            }
        }

        //printing points at the end of bridges whose value is 2
        Debug.Log("The below are brigde end points");
        foreach (Vector3 x in bridgeNodes)
        {
            Debug.Log(x);
        }

        foreach (Vector3 x in bridgeNodeList())
        {
            Debug.Log("Bridge Node List" + x);
        }

        //creating graph by adding edges and nodes to the graph
        graph = new Graph();
        graph.addArrayToGraph(bridgeNodes);
        foreach (Vector3 x in definedPoints)
        {
            Vector3 left = x + new Vector3(-1f, 0f, 0f);
            Vector3 right = x + new Vector3(1f, 0f, 0f);
            Vector3 topleft = x + new Vector3(-0.5f, 0f, 1f);
            Vector3 topright = x + new Vector3(0.5f, 0f, 1f);
            Vector3 bottomleft = x + new Vector3(-0.5f, 0f, -1f);
            Vector3 bottomright = x + new Vector3(0.5f, 0f, -1f);
            if (definedPoints.Contains(left))
            {
                graph.insert_edge(x, left);
            }
            if (definedPoints.Contains(right))
            {
                graph.insert_edge(x, right);
            }
            if (definedPoints.Contains(topleft))
            {
                graph.insert_edge(x, topleft);
            }
            if (definedPoints.Contains(topright))
            {
                graph.insert_edge(x, topright);
            }
            if (definedPoints.Contains(bottomleft))
            {
                graph.insert_edge(x, bottomleft);
            }
            if (definedPoints.Contains(bottomright))
            {
                graph.insert_edge(x, bottomright);
            }
        }

        foreach (Vector3 point in players[1])
        {
            botsvp.Add(point);
        }


        //		Debug.Log (bridgeNodes.ToString());

        //		ArrayList edge_list = new ArrayList ();
        //		edge_list = graph.get_edge_list ();
        //		print ("Edge_list " + graph.get_edge_list());
        //		foreach (ArrayList x in edge_list) {
        //			Debug.Log ("Edge_list " + x);
        //		}
        //
        //		foreach (ArrayList x in graph.get_edge_list()) {
        //			foreach (string y in x) {
        //				Debug.Log ("List " + y);
        //			}
        //		}

        //		Vector3 hub = new Vector3 (0f, 1f, 0f);
        //
        //		Dictionary<Vector3, int> distance_to_points = new Dictionary<Vector3, int> ();
        //		distance_to_points = shortest_distances (graph, hub);
        //
        //		Debug.Log ("Length of diction dtp " + distance_to_points.Count);
        //
        //		foreach (Vector3 x in distance_to_points.Keys) {
        //			Debug.Log ("distance to the node from source in dictionary: " + x + distance_to_points [x]); 
        //		}

    }


    void Update() {

        //Restarts the game if "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        //changes the players turn if "C" key is pressed
        if (Input.GetKeyDown(KeyCode.C))
        {
            changeTurn();
        }

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        switch (turn)
        {
            case 0:
                if (checkAllPlayersHaveHub() == false)
                {
                    disableTrackInstantiation();

                    //				Dictionary<Vector3, int> distances_to_victory_points = new Dictionary<Vector3, int> ();
                    //				foreach (Vector3 x in definedPoints) {
                    //					Dictionary<Vector3, int> d_to_vps = shortest_distances (graph, x);
                    //					int total_distance = new int ();
                    //					foreach (Vector3 y in players[2]) {
                    //						total_distance += d_to_vps [y];
                    //					}
                    //					distances_to_victory_points [x] = total_distance;
                    //				}
                    //
                    //				Vector3 hub_point = shortest_distance_node (distances_to_victory_points);
                    //
                    //				foreach (Vector3 x in playersHubCollections) {
                    //					if (x == hub_point) {
                    //						distances_to_victory_points.Remove (hub_point);
                    //						hub_point = shortest_distance_node (distances_to_victory_points);
                    //					}
                    //				}
                    //
                    //				Debug.Log ("The best point for bot to place the hub is: " + hub_point);
                    //
                    //				foreach (Vector3 x in distances_to_victory_points.Keys) {
                    //					Debug.Log ("Total distance to all victory points from " + x + " is " + distances_to_victory_points [x]);
                    //				}

                    if (placeHub(turn))
                    {
                        changeTurn();
                    }
                }
                else
                {

                    try
                    {
                        addPlayersCollectionsIfConnected(1);
                    }
                    catch
                    {
                        return;
                    }

                    if (checkIfAnyWinner())
                    {
                        disableTrackInstantiation();
                        gameOverText.text = "Player " + winner() + " wins!";
                        foreach (Vector3 y in players[1])
                        {
                            GameObject p2 = Instantiate(p2vp, y + new Vector3(0f, -0.3f, 0.3f), Quaternion.identity) as GameObject;
                        }
                    }
                    else
                    {

                        placetrack(turn);

                        try
                        {
                            addPlayersCollectionsIfConnected(1);
                        }
                        catch
                        {
                            return;
                        }
                        //				for (int i = 0; i < maxHubs && i != 0; i++) {
                        //					foreach (Vector3 x in playersCollections[0]) {
                        //						if (playersCollections [i].Contains (x)) {
                        //							foreach (Vector3 point in playersCollections[0]) {
                        //								if (playersCollections [i].Contains (point) == false) {
                        //									playersCollections [i].Add (point);
                        //								}
                        //							}
                        //							foreach (Vector3 point in playersCollections[i]) {
                        //								if (playersCollections [0].Contains (point) == false) {
                        //									playersCollections [0].Add (point);
                        //								}
                        //							}
                        //						}
                        //					}
                        //				}
                        if (layedTracks == 2)
                        {
                            changeTurn();
                        }
                    }
                }


                break;

            //		case 1:
            //			if (checkAllPlayersHaveHub () == false) {
            //				disableTrackInstantiation ();
            //
            ////				Dictionary<Vector3, int> distances_to_victory_points = new Dictionary<Vector3, int> ();
            ////				foreach (Vector3 x in definedPoints) {
            ////					Dictionary<Vector3, int> d_to_vps = shortest_distances (graph, x);
            ////					int total_distance = new int ();
            ////					foreach (Vector3 y in players[2]) {
            ////						total_distance += d_to_vps [y];
            ////					}
            ////					distances_to_victory_points [x] = total_distance;
            ////				}
            ////
            ////				Vector3 hub_point = shortest_distance_node (distances_to_victory_points);
            ////
            ////				foreach (Vector3 x in playersHubCollections) {
            ////					if (x == hub_point) {
            ////						distances_to_victory_points.Remove (hub_point);
            ////						hub_point = shortest_distance_node (distances_to_victory_points);
            ////					}
            ////				}
            ////
            ////				Debug.Log ("The best point for bot to place the hub is: " + hub_point);
            ////
            ////				foreach (Vector3 x in distances_to_victory_points.Keys) {
            ////					Debug.Log ("Total distance to all victory points from " + x + " is " + distances_to_victory_points [x]);
            ////				}
            //
            //				if (placeHub (turn)) {
            //					changeTurn ();
            //				}
            //			} else {
            //
            //				try {
            //					addPlayersCollectionsIfConnected (1);
            //				}
            //				catch {
            //					return;
            //				}
            //
            //
            //				if (checkIfAnyWinner ()) {
            //					disableTrackInstantiation ();
            //					gameOverText.text = "Player " + winner () + " wins!";
            //				} else {
            //					
            //					if (Physics.Raycast (ray, out hit)) {
            //						if (Input.GetMouseButtonDown (0)) {
            //							if (hit.collider.gameObject.tag == "dLine") {
            //								float z = hit.collider.gameObject.transform.position.z;
            //								float x = hit.collider.gameObject.transform.position.x;
            //
            //								Vector3 p1 = new Vector3 (x + 0.25f, 1f, z - 0.5f);
            //								Vector3 p2 = new Vector3 (x - 0.25f, 1f, z + 0.5f);
            //
            //								Debug.Log (p1);
            //								Debug.Log (p2);
            //
            //								if (playersCollections [1].Contains (p1) || playersCollections [1].Contains (p2)) {
            //									if (playersCollections [1].Contains (p1)) {
            //										playersCollections [1].Add (p2);
            //									} else {
            //										playersCollections [1].Add (p1);
            //									}
            //									Debug.Log ("Players checked if connected");
            //									GameObject block = Instantiate (TrackBlocks, hit.collider.gameObject.transform.position + new Vector3 (0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
            //									layedTracks += 1;
            //									UpdateCount ();
            //									try {
            //										addPlayersCollectionsIfConnected (1);
            //									}
            //									catch {
            //										return;
            //									}
            //								}
            //								foreach (Vector3 p in playersCollections[1]) {
            //									Debug.Log ("player2sCollection points: " + p);
            //								}
            //							} else if (hit.collider.gameObject.tag == "hLine") {
            //								float z = hit.collider.gameObject.transform.position.z;
            //								float x = hit.collider.gameObject.transform.position.x;
            //
            //								Vector3 p1 = new Vector3 (x + 0.5f, 1f, z);
            //								Vector3 p2 = new Vector3 (x - 0.5f, 1f, z);
            //
            //								Debug.Log (p1);
            //								Debug.Log (p2);
            //
            //								if (playersCollections [1].Contains (p1) || playersCollections [1].Contains (p2)) {
            //									if (playersCollections [1].Contains (p1)) {
            //										playersCollections [1].Add (p2);
            //									} else {
            //										playersCollections [1].Add (p1);
            //									}
            //									Debug.Log ("Players checked if connected");
            //									GameObject block = Instantiate (TrackBlocks, hit.collider.gameObject.transform.position + new Vector3 (0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
            //									layedTracks += 1;
            //									UpdateCount ();
            //									try {
            //										addPlayersCollectionsIfConnected (1);
            //									}
            //									catch {
            //										return;
            //									}
            //								}
            //								foreach (Vector3 p in playersCollections[1]) {
            //									Debug.Log ("player2sCollection points: " + p);
            //								}
            //							} else if (hit.collider.gameObject.tag == "vLine") {
            //								float z = hit.collider.gameObject.transform.position.z;
            //								float x = hit.collider.gameObject.transform.position.x;
            //
            //								Vector3 p1 = new Vector3 (x - 0.25f, 1f, z - 0.5f);
            //								Vector3 p2 = new Vector3 (x + 0.25f, 1f, z + 0.5f);
            //
            //								Debug.Log (p1);
            //								Debug.Log (p2);
            //
            //								if (playersCollections [1].Contains (p1) || playersCollections [1].Contains (p2)) {
            //									if (playersCollections [1].Contains (p1)) {
            //										playersCollections [1].Add (p2);
            //									} else {
            //										playersCollections [1].Add (p1);
            //									}
            //									Debug.Log ("Players checked if connected");
            //									GameObject block = Instantiate (TrackBlocks, hit.collider.gameObject.transform.position + new Vector3 (0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
            //									layedTracks += 1;
            //									UpdateCount ();
            //									try {
            //										addPlayersCollectionsIfConnected (1);
            //									}
            //									catch {
            //										return;
            //									}
            //								}
            //								foreach (Vector3 p in playersCollections[1]) {
            //									Debug.Log ("player2sCollection points: " + p);
            //								}
            //							} else if (hit.collider.gameObject.tag == "dBridge" && layedTracks == 0) {
            //								float z = hit.collider.gameObject.transform.position.z;
            //								float x = hit.collider.gameObject.transform.position.x;
            //
            //								Vector3 p1 = new Vector3 (x + 0.25f, 1f, z - 0.5f);
            //								Vector3 p2 = new Vector3 (x - 0.25f, 1f, z + 0.5f);
            //
            //								Debug.Log (p1);
            //								Debug.Log (p2);
            //
            //								if (playersCollections [1].Contains (p1) || playersCollections [1].Contains (p2)) {
            //									if (playersCollections [1].Contains (p1)) {
            //										playersCollections [1].Add (p2);
            //									} else {
            //										playersCollections [1].Add (p1);
            //									}
            //									Debug.Log ("Players checked if connected");
            //									GameObject block = Instantiate (TrackBlocks, hit.collider.gameObject.transform.position + new Vector3 (0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
            //									layedTracks += 2;
            //									UpdateCount ();
            //									try {
            //										addPlayersCollectionsIfConnected (1);
            //									}
            //									catch {
            //										return;
            //									}
            //								}
            //								foreach (Vector3 p in playersCollections[1]) {
            //									Debug.Log ("player2sCollection points: " + p);
            //								}
            //							} else if (hit.collider.gameObject.tag == "hBridge" && layedTracks == 0) {
            //								float z = hit.collider.gameObject.transform.position.z;
            //								float x = hit.collider.gameObject.transform.position.x;
            //
            //								Vector3 p1 = new Vector3 (x + 0.5f, 1f, z);
            //								Vector3 p2 = new Vector3 (x - 0.5f, 1f, z);
            //
            //								Debug.Log (p1);
            //								Debug.Log (p2);
            //
            //								if (playersCollections [1].Contains (p1) || playersCollections [1].Contains (p2)) {
            //									if (playersCollections [1].Contains (p1)) {
            //										playersCollections [1].Add (p2);
            //									} else {
            //										playersCollections [1].Add (p1);
            //									}
            //									Debug.Log ("Players checked if connected");
            //									GameObject block = Instantiate (TrackBlocks, hit.collider.gameObject.transform.position + new Vector3 (0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
            //									layedTracks += 2;
            //									UpdateCount ();
            //									try {
            //										addPlayersCollectionsIfConnected (1);
            //									}
            //									catch {
            //										return;
            //									}
            //								}
            //								foreach (Vector3 p in playersCollections[1]) {
            //									Debug.Log ("player2sCollection points: " + p);
            //								}
            //							} else if (hit.collider.gameObject.tag == "vBridge" && layedTracks == 0) {
            //								float z = hit.collider.gameObject.transform.position.z;
            //								float x = hit.collider.gameObject.transform.position.x;
            //
            //								Vector3 p1 = new Vector3 (x - 0.25f, 1f, z - 0.5f);
            //								Vector3 p2 = new Vector3 (x + 0.25f, 1f, z + 0.5f);
            //
            //								Debug.Log (p1);
            //								Debug.Log (p2);
            //
            //								if (playersCollections [1].Contains (p1) || playersCollections [1].Contains (p2)) {
            //									if (playersCollections [1].Contains (p1)) {
            //										playersCollections [1].Add (p2);
            //									} else {
            //										playersCollections [1].Add (p1);
            //									}
            //									Debug.Log ("Players checked if connected");
            //									GameObject block = Instantiate (TrackBlocks, hit.collider.gameObject.transform.position + new Vector3 (0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
            //									layedTracks += 2;
            //									UpdateCount ();
            //									try {
            //										addPlayersCollectionsIfConnected (1);
            //									}
            //									catch {
            //										return;
            //									}
            //								}
            //								foreach (Vector3 p in playersCollections[1]) {
            //									Debug.Log ("player2sCollection points: " + p);
            //								}
            //							}
            //							try {
            //								addPlayersCollectionsIfConnected (1);
            //							}
            //							catch {
            //								return;
            //							}
            //						}
            //					}
            //					try {
            //						addPlayersCollectionsIfConnected (1);
            //					}
            //					catch {
            //						return;
            //					}
            ////				for (int i = 0; i < maxHubs && i != 1; i++) {
            ////					foreach (Vector3 x in playersCollections[1]) {
            ////						if (playersCollections [i].Contains (x)) {
            ////							foreach (Vector3 point in playersCollections[1]) {
            ////								if (playersCollections [i].Contains (point) == false) {
            ////									playersCollections [i].Add (point);
            ////								}
            ////							}
            ////							foreach (Vector3 point in playersCollections[i]) {
            ////								if (playersCollections [1].Contains (point) == false) {
            ////									playersCollections [1].Add (point);
            ////								}
            ////							}
            ////						}
            ////					}
            ////				}
            //					if (layedTracks == 2) {
            //						//					layedTracks = 0;
            //						//					UpdateCount ();
            //						changeTurn ();
            //						//					if (turn + 1 < maxHubs) {
            //						//						turn = 2;
            //						//						UpdateTurn ();
            //						//					} else {
            //						//						turn = 0;
            //						//						UpdateTurn ();
            //						//					}
            //					}
            //				}
            //			}
            //			break;

            case 1:
                if (checkAllPlayersHaveHub() == false)
                {
                    disableTrackInstantiation();


                    Dictionary<Vector3, int> distances_to_victory_points = new Dictionary<Vector3, int>();
                    foreach (Vector3 x in definedPoints)
                    {
                        Dictionary<Vector3, int> d_to_vps = shortest_distances(graph, x);
                        int total_distance = new int();
                        foreach (Vector3 y in players[1])
                        {
                            total_distance += d_to_vps[y];
                        }
                        distances_to_victory_points[x] = total_distance;
                    }

                    Vector3 hub_point = shortest_distance_node(distances_to_victory_points);

                    foreach (Vector3 x in playersHubCollections)
                    {
                        if (x == hub_point)
                        {
                            distances_to_victory_points.Remove(hub_point);
                            hub_point = shortest_distance_node(distances_to_victory_points);
                        }
                    }

                    Debug.Log("The best point for bot to place the hub is: " + hub_point);

                    foreach (Vector3 x in distances_to_victory_points.Keys)
                    {
                        Debug.Log("Total distance to all victory points from " + x + " is " + distances_to_victory_points[x]);
                    }

                    GameObject bots_hub = Instantiate(userHubs3, hub_point + new Vector3(0f, 0.25f, 0f), Quaternion.identity) as GameObject;
                    playersCollections[1].Add(hub_point);
                    playersHubCollections.Add(hub_point);
                    hubCount++;
                    changeTurn();

                }
                else
                {
                    if (checkIfAnyWinner())
                    {
                        disableTrackInstantiation();
                        gameOverText.text = "Player " + winner() + " wins!";
                        foreach (Vector3 y in players[1])
                        {
                            GameObject p2 = Instantiate(p2vp, y + new Vector3(0f, -0.3f, 0.3f), Quaternion.identity) as GameObject;
                        }
                    }
                    else
                    {

                        //				addPlayersCollectionsIfConnected (2);

                        Vector3 current_point = new Vector3();
                        Vector3 hub_point = (Vector3)playersCollections[1][0];
                        Debug.Log("hub_points is: " + hub_point);

                        Dictionary<Vector3, int> distance_to_vpoints = new Dictionary<Vector3, int>();


                        if (playersCollections[1].Count == 1)
                        {
                            current_point = hub_point;
                            Debug.Log("current_point for bot " + current_point);
                        }
                        else
                        {
                            current_point = get_current_point();
                            Debug.Log("current_point for bot " + current_point);
                        }

                        Dictionary<Vector3, List<Vector3>> path_to_dest = shortest_path(graph, current_point);

                        foreach (Vector3 x in path_to_dest.Keys)
                        {
                            //					Debug.Log ("Keys are " + x);
                            foreach (Vector3 y in path_to_dest[x])
                            {
                                Debug.Log("Route to " + x + " is " + y);
                            }
                        }


                        Vector3 track_position = new Vector3();
                        Vector3 pt = new Vector3();
                        foreach (Vector3 x in path_to_dest.Keys)
                        {
                            if (path_to_dest[x].Count == 1)
                            {
                                pt = (Vector3)path_to_dest[x][0];
                                Debug.Log("Point pt is " + pt);
                                path_to_dest[x].RemoveAt(0);
                            }
                            else
                            {
                                pt = (Vector3)path_to_dest[x][(path_to_dest[x].Count) - 1];
                                Debug.Log("Point pt is " + pt);
                                path_to_dest[x].RemoveAt((path_to_dest[x].Count) - 1);
                            }
                        }

                        if (pt.z == current_point.z)
                        {
                            track_position = new Vector3((pt.x + current_point.x) / 2f, current_point.y, current_point.z);
                        }
                        else
                        {
                            if (pt.x < current_point.x)
                            {
                                track_position = new Vector3((pt.x + 0.25f), 1f, (pt.z + current_point.z) / 2f);
                            }
                            if (pt.x > current_point.x)
                            {
                                track_position = new Vector3((pt.x - 0.25f), 1f, (pt.z + current_point.z) / 2f);
                            }
                        }

                        track_position.y += 10f;
                        Debug.Log("Track position would be " + track_position.ToString("F2"));
                        if (Physics.Raycast(track_position, Vector3.down, out hit, 10))
                        {
                            Debug.Log("------------->");
                            if (hit.collider.gameObject.tag == "dLine")
                            {
                                Debug.Log("Ray hits the line");
                                float z = hit.collider.gameObject.transform.position.z;
                                float x = hit.collider.gameObject.transform.position.x;

                                Debug.Log("Made it to through the point calculations");

                                Vector3 p1 = new Vector3(x + 0.25f, 1f, z - 0.5f);
                                Vector3 p2 = new Vector3(x - 0.25f, 1f, z + 0.5f);

                                Debug.Log(p1.ToString("F2"));
                                Debug.Log(p2.ToString("F2"));
                                //adds one point to the collection since the other already exist
                                //checks what point already exist and adds the other
                                if (playersCollections[1].Contains(p1) || playersCollections[1].Contains(p2))
                                {
                                    if (playersCollections[1].Contains(p1))
                                    {
                                        playersCollections[1].Add(p2);
                                    }
                                    else
                                    {
                                        playersCollections[1].Add(p1);
                                    }
                                    Debug.Log("Made it to the diagonal placement");
                                    //Places Track
                                    GameObject block = Instantiate(TrackBlocks, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
                                    //Increments the counter
                                    layedTracks += 1;
                                    Debug.Log("Tracks layed " + layedTracks);
                                    UpdateCount();
                                    //shares players collections if connected
                                    try
                                    {
                                        addPlayersCollectionsIfConnected(1);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    finally
                                    {
                                        //prints out all the poins in players collections
                                        foreach (Vector3 p in playersCollections[1])
                                        {
                                            Debug.Log("player3sCollection points: " + p);
                                        }
                                        if (layedTracks == 2)
                                        {
                                            changeTurn();
                                        }
                                    }
                                }

                            }
                            else if (hit.collider.gameObject.tag == "hLine")
                            {
                                Debug.Log("Ray hits the line");
                                float z = hit.collider.gameObject.transform.position.z;
                                float x = hit.collider.gameObject.transform.position.x;

                                Vector3 p1 = new Vector3(x + 0.5f, 1f, z);
                                Vector3 p2 = new Vector3(x - 0.5f, 1f, z);

                                Debug.Log("Made it to through the point calculations");

                                Debug.Log(p1.ToString("F2"));
                                Debug.Log(p2.ToString("F2"));

                                if (playersCollections[1].Contains(p1) || playersCollections[1].Contains(p2))
                                {
                                    if (playersCollections[1].Contains(p1))
                                    {
                                        playersCollections[1].Add(p2);
                                    }
                                    else
                                    {
                                        playersCollections[1].Add(p1);
                                    }
                                    Debug.Log("Made it to the horizontal placement");
                                    GameObject block = Instantiate(TrackBlocks, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
                                    layedTracks += 1;
                                    Debug.Log("Tracks layed " + layedTracks);
                                    UpdateCount();
                                    try
                                    {
                                        addPlayersCollectionsIfConnected(1);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    finally
                                    {
                                        //prints out all the poins in players collections
                                        foreach (Vector3 p in playersCollections[1])
                                        {
                                            Debug.Log("player3sCollection points: " + p);
                                        }
                                        if (layedTracks == 2)
                                        {
                                            changeTurn();
                                        }
                                    }
                                }
                                //							foreach (Vector3 p in playersCollections[2]) {
                                //								Debug.Log ("player3sCollection points: " + p);
                                //							}
                                //							if (layedTracks == 2) {
                                //								changeTurn ();
                                //							}
                            }
                            else if (hit.collider.gameObject.tag == "vLine")
                            {
                                Debug.Log("Ray hits the line");
                                float z = hit.collider.gameObject.transform.position.z;
                                float x = hit.collider.gameObject.transform.position.x;

                                Debug.Log("Made it to through the point calculations");

                                Vector3 p1 = new Vector3(x - 0.25f, 1f, z - 0.5f);
                                Vector3 p2 = new Vector3(x + 0.25f, 1f, z + 0.5f);

                                Debug.Log(p1.ToString("F2"));
                                Debug.Log(p2.ToString("F2"));

                                if (playersCollections[1].Contains(p1) || playersCollections[1].Contains(p2))
                                {
                                    if (playersCollections[1].Contains(p1))
                                    {
                                        playersCollections[1].Add(p2);
                                    }
                                    else
                                    {
                                        playersCollections[1].Add(p1);
                                    }
                                    Debug.Log("Made it to the vertical placement");
                                    GameObject block = Instantiate(TrackBlocks, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
                                    layedTracks += 1;
                                    Debug.Log("Tracks layed " + layedTracks);
                                    UpdateCount();
                                    try
                                    {
                                        addPlayersCollectionsIfConnected(1);
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                    finally
                                    {
                                        //prints out all the poins in players collections
                                        foreach (Vector3 p in playersCollections[1])
                                        {
                                            Debug.Log("player3sCollection points: " + p);
                                        }
                                        if (layedTracks == 2)
                                        {
                                            changeTurn();
                                        }
                                    }
                                }
                                //							foreach (Vector3 p in playersCollections[2]) {
                                //								Debug.Log ("player3sCollection points: " + p);
                                //							}
                                //							if (layedTracks == 2) {
                                //								changeTurn ();
                                //							}
                            }
                            else if (hit.collider.gameObject.tag == "dBridge")
                            {
                                if (layedTracks == 1)
                                {
                                    changeTurn();
                                }
                                else if (layedTracks == 0)
                                {
                                    Debug.Log("Ray hits the line");
                                    float z = hit.collider.gameObject.transform.position.z;
                                    float x = hit.collider.gameObject.transform.position.x;

                                    Debug.Log("Made it to through the point calculations");

                                    Vector3 p1 = new Vector3(x + 0.25f, 1f, z - 0.5f);
                                    Vector3 p2 = new Vector3(x - 0.25f, 1f, z + 0.5f);

                                    Debug.Log(p1.ToString("F2"));
                                    Debug.Log(p2.ToString("F2"));

                                    if (playersCollections[1].Contains(p1) || playersCollections[1].Contains(p2))
                                    {
                                        if (playersCollections[1].Contains(p1))
                                        {
                                            playersCollections[1].Add(p2);
                                        }
                                        else
                                        {
                                            playersCollections[1].Add(p1);
                                        }
                                        Debug.Log("Made it to the diagonal bridge placement");
                                        GameObject block = Instantiate(TrackBlocks, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
                                        layedTracks += 2;
                                        Debug.Log("Tracks layed " + layedTracks);
                                        UpdateCount();
                                        try
                                        {
                                            addPlayersCollectionsIfConnected(1);
                                        }
                                        catch
                                        {
                                            return;
                                        }
                                        finally
                                        {
                                            //prints out all the poins in players collections
                                            foreach (Vector3 p in playersCollections[1])
                                            {
                                                Debug.Log("player3sCollection points: " + p);
                                            }
                                            if (layedTracks == 2)
                                            {
                                                changeTurn();
                                            }
                                        }
                                    }
                                }
                                //							foreach (Vector3 p in playersCollections[2]) {
                                //								Debug.Log ("player3sCollection points: " + p);
                                //							}
                                //							if (layedTracks == 2) {
                                //								changeTurn ();
                                //							}
                            }
                            else if (hit.collider.gameObject.tag == "hBridge")
                            {
                                if (layedTracks == 1)
                                {
                                    changeTurn();
                                }
                                else if (layedTracks == 0)
                                {
                                    Debug.Log("Ray hits the line");
                                    float z = hit.collider.gameObject.transform.position.z;
                                    float x = hit.collider.gameObject.transform.position.x;

                                    Debug.Log("Made it to through the point calculations");

                                    Vector3 p1 = new Vector3(x + 0.5f, 1f, z);
                                    Vector3 p2 = new Vector3(x - 0.5f, 1f, z);

                                    Debug.Log(p1.ToString("F2"));
                                    Debug.Log(p2.ToString("F2"));

                                    if (playersCollections[1].Contains(p1) || playersCollections[1].Contains(p2))
                                    {
                                        if (playersCollections[1].Contains(p1))
                                        {
                                            playersCollections[1].Add(p2);
                                        }
                                        else
                                        {
                                            playersCollections[1].Add(p1);
                                        }
                                        Debug.Log("Made it to the horizontal bridge placement");
                                        GameObject block = Instantiate(TrackBlocks, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
                                        layedTracks += 2;
                                        Debug.Log("Tracks layed " + layedTracks);
                                        UpdateCount();
                                        try
                                        {
                                            addPlayersCollectionsIfConnected(1);
                                        }
                                        catch
                                        {
                                            return;
                                        }
                                        finally
                                        {
                                            //prints out all the poins in players collections
                                            foreach (Vector3 p in playersCollections[1])
                                            {
                                                Debug.Log("player3sCollection points: " + p);
                                            }
                                            if (layedTracks == 2)
                                            {
                                                changeTurn();
                                            }
                                        }
                                    }
                                }
                                //							foreach (Vector3 p in playersCollections[2]) {
                                //								Debug.Log ("player3sCollection points: " + p);
                                //							}
                                //							if (layedTracks == 2) {
                                //								changeTurn ();
                                //							}
                            }
                            else if (hit.collider.gameObject.tag == "vBridge")
                            {
                                if (layedTracks == 1)
                                {
                                    changeTurn();
                                }
                                else if (layedTracks == 0)
                                {
                                    Debug.Log("Ray hits the line");
                                    float z = hit.collider.gameObject.transform.position.z;
                                    float x = hit.collider.gameObject.transform.position.x;

                                    Debug.Log("Made it to through the point calculations");

                                    Vector3 p1 = new Vector3(x - 0.25f, 1f, z - 0.5f);
                                    Vector3 p2 = new Vector3(x + 0.25f, 1f, z + 0.5f);

                                    Debug.Log(p1.ToString("F2"));
                                    Debug.Log(p2.ToString("F2"));

                                    if (playersCollections[1].Contains(p1) || playersCollections[1].Contains(p2))
                                    {
                                        if (playersCollections[1].Contains(p1))
                                        {
                                            playersCollections[1].Add(p2);
                                        }
                                        else
                                        {
                                            playersCollections[1].Add(p1);
                                        }
                                        Debug.Log("Made it to the vertical bridge placement");
                                        GameObject block = Instantiate(TrackBlocks, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
                                        layedTracks += 2;
                                        Debug.Log("Tracks layed " + layedTracks);
                                        UpdateCount();
                                        try
                                        {
                                            addPlayersCollectionsIfConnected(1);
                                        }
                                        catch
                                        {
                                            return;
                                        }
                                        finally
                                        {
                                            //prints out all the poins in players collections
                                            foreach (Vector3 p in playersCollections[1])
                                            {
                                                Debug.Log("player3sCollection points: " + p);
                                            }
                                            if (layedTracks == 2)
                                            {
                                                changeTurn();
                                            }
                                        }
                                    }
                                }
                                //							foreach (Vector3 p in playersCollections[2]) {
                                //								Debug.Log ("player3sCollection points: " + p);
                                //							}
                                //							if (layedTracks == 2) {
                                //								changeTurn ();
                                //							}
                            }

                            //					for (int i = 0; i < maxHubs && i != 2; i++) {
                            //						foreach (Vector3 x in playersCollections[2]) {
                            //							if (playersCollections [i].Contains (x)) {
                            //								foreach (Vector3 point in playersCollections[2]) {
                            //									if (playersCollections [i].Contains (point) == false) {
                            //										playersCollections [i].Add (point);
                            //									}
                            //								}
                            //								foreach (Vector3 point in playersCollections[i]) {
                            //									if (playersCollections [2].Contains (point) == false) {
                            //										playersCollections [2].Add (point);
                            //									}
                            //								}
                            //							}
                            //						}
                            //					}
                            try
                            {
                                foreach (Vector3 vp in botsvp)
                                {
                                    if (playersCollections[1].Contains(vp))
                                    {
                                        Debug.Log("vp " + vp);
                                        botsvp.Remove(vp);
                                    }
                                }
                            }
                            catch
                            {
                                return;
                            }
                        }
                    }
                }

                break;
        }
    }


    bool placeHub(int turn)
    {
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.gameObject.tag == "Point")
                {
                    if (turn == 0)
                    {
                        GameObject startingPoint = Instantiate(userHubs1, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), Quaternion.identity) as GameObject;
                        Vector3 hub1 = hit.collider.gameObject.transform.position;
                        playersCollections[0].Insert(0, hit.collider.gameObject.transform.position);
                        playersHubCollections.Add(hit.collider.gameObject.transform.position);
                        hubCount++;
                        return true;
                    }
                    //					if (turn == 1) {
                    //						GameObject startingPoint = Instantiate (userHubs2, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f) , Quaternion.identity) as GameObject;
                    //						Vector3 hub2 = hit.collider.gameObject.transform.position;
                    //						playersCollections[1].Insert (0, hit.collider.gameObject.transform.position);
                    //						playersHubCollections.Add (hit.collider.gameObject.transform.position);
                    //						hubCount++;
                    //						return true;
                    //					}
                    //					if (turn == 2) {
                    //						GameObject startingPoint = Instantiate (userHubs3, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f) , Quaternion.identity) as GameObject;
                    //						Vector3 hub3 = hit.collider.gameObject.transform.position;
                    //						playersCollections[2].Insert (0, hit.collider.gameObject.transform.position);						
                    //						hubCount++;
                    //						return true;
                    //					}
                    if (turn == 3)
                    {
                        GameObject startingPoint = Instantiate(userHubs4, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), Quaternion.identity) as GameObject;
                        Vector3 hub4 = hit.collider.gameObject.transform.position;
                        playersCollections[3].Insert(0, hit.collider.gameObject.transform.position);
                        playersHubCollections.Add(hit.collider.gameObject.transform.position);
                        hubCount++;
                        return true;
                    }
                    if (turn == 4)
                    {
                        GameObject startingPoint = Instantiate(userHubs5, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), Quaternion.identity) as GameObject;
                        Vector3 hub5 = hit.collider.gameObject.transform.position;
                        playersCollections[4].Insert(0, hit.collider.gameObject.transform.position);
                        playersHubCollections.Add(hit.collider.gameObject.transform.position);
                        hubCount++;
                        return true;
                    }
                    //					hubCount++;
                }
            }
        }
        return false;
    }

    // changes turns
    void changeTurn()
    {
        layedTracks = 0;
        UpdateCount();
        if (turn + 1 < maxHubs)
        {
            turn += 1;
            UpdateTurn();
        }
        else
        {
            turn = 0;
            UpdateTurn();
        }
    }

    // return the winner
    int winner()
    {
        for (int i = 0; i < maxHubs; i++)
        {
            if (playersCollections[i].Count > 1)
            {
                if (playersCollections[i].Contains(players[i][0]) && playersCollections[i].Contains(players[i][1]) && playersCollections[i].Contains(players[i][2]) && playersCollections[i].Contains(players[i][3]))
                {
                    return i + 1;
                }
            }
        }
        return 0;
    }

    // checks if any winner
    bool checkIfAnyWinner()
    {
        //		Debug.Log ("entered check winner loop");
        for (int i = 0; i < maxHubs; i++)
        {
            //			Debug.Log ("Checking for player " + i);
            foreach (Vector3 x in players[i])
            {
                //				Debug.Log ("player " + i + " vps: " + x);
            }
            if (playersCollections[i].Count > 1)
            {
                if (playersCollections[i].Contains(players[i][0]) && playersCollections[i].Contains(players[i][1]) && playersCollections[i].Contains(players[i][2]) && playersCollections[i].Contains(players[i][3]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // shows how many moves left for the player
    void UpdateCount()
    {
        if (layedTracks == 0)
        {
            count = 2;
        }
        else if (layedTracks == 1)
        {
            count = 1;
        }
        movesText.text = "Moves left: " + count;
    }

    // updates turn
    void UpdateTurn()
    {
        switch (turn)
        {
            case 0:
                player = "Player 1";
                break;
            case 1:
                player = "Player 2";
                break;
            case 2:
                player = "Player 3";
                break;
            case 3:
                player = "Player 4";
                break;
            case 4:
                player = "Player 5";
                break;
            case 5:
                player = "Player 6";
                break;
        }
        playersTurnText.text = "Turn: " + player;
    }


    void getHubs()
    {
        disableTrackInstantiation();

        if (hubCount < maxHubs)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider.gameObject.tag == "Point")
                    {

                        if (hubCount == 0)
                        {
                            GameObject startingPoint = Instantiate(userHubs1, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), Quaternion.identity) as GameObject;
                            Vector3 hub1 = hit.collider.gameObject.transform.position;
                            playersCollections[0].Insert(0, hit.collider.gameObject.transform.position);
                        }
                        if (hubCount == 1)
                        {
                            GameObject startingPoint = Instantiate(userHubs2, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), Quaternion.identity) as GameObject;
                            Vector3 hub2 = hit.collider.gameObject.transform.position;
                            playersCollections[1].Insert(0, hit.collider.gameObject.transform.position);
                        }
                        if (hubCount == 2)
                        {
                            GameObject startingPoint = Instantiate(userHubs3, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), Quaternion.identity) as GameObject;
                            Vector3 hub3 = hit.collider.gameObject.transform.position;
                            playersCollections[2].Insert(0, hit.collider.gameObject.transform.position);
                        }
                        if (hubCount == 3)
                        {
                            GameObject startingPoint = Instantiate(userHubs4, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), Quaternion.identity) as GameObject;
                            Vector3 hub4 = hit.collider.gameObject.transform.position;
                            playersCollections[3].Insert(0, hit.collider.gameObject.transform.position);
                        }
                        if (hubCount == 4)
                        {
                            GameObject startingPoint = Instantiate(userHubs5, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), Quaternion.identity) as GameObject;
                            Vector3 hub5 = hit.collider.gameObject.transform.position;
                            playersCollections[4].Insert(0, hit.collider.gameObject.transform.position);
                        }
                        hubCount++;
                        //						Debug.Log ("hub position: " + startingPoint.transform.position);
                        Debug.Log("collider position: " + hit.collider.gameObject.transform.position.ToString("F4"));
                    }
                }
            }
        }

    }


    // Generates the board map
    void GenerateRhombicalGrid2()
    {
        horizontalLines = new Vector3[xSize * (zSize + 1)];
        Quaternion horizontalLine = Quaternion.Euler(90f, 90f, 0f);

        verticalLines = new Vector3[(xSize + 1) * zSize];
        Quaternion verticalLine = Quaternion.Euler(90f, 26.5f, 0f);

        diagonalLines = new Vector3[xSize * zSize];
        Quaternion diagonalLine = Quaternion.Euler(90f, -26.5f, 0f);

        Vector3 vertexOffset = new Vector3(0.5f, 0f, 0f);
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        Vector3 verticalLineOffset = new Vector3(0.25f, 0f, 0.5f);
        Vector3 diagonalLineOffset = new Vector3(-0.25f, 0f, 0.5f);


        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, 1f, z);
                if (delete_vertices.Contains(vertices[i]))
                {
                    continue;
                }
                else
                {
                    GameObject vertex = Instantiate(point, vertices[i] + (z * vertexOffset), Quaternion.identity) as GameObject;
                    if (city_zones.ContainsKey(vertices[i]))
                    {
                        if (city_zones[vertices[i]] == "zone1")
                        {
                            GameObject green_cities = Instantiate(greenCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone1.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone1_full")
                        {
                            GameObject green_cities = Instantiate(greenCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone1_full.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone2")
                        {
                            GameObject blue_cities = Instantiate(blueCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone2.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone2_full")
                        {
                            GameObject blue_cities = Instantiate(blueCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone2_full.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone3")
                        {
                            GameObject red_cities = Instantiate(redCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone3.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone3_full")
                        {
                            GameObject red_cities = Instantiate(redCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone3_full.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone4")
                        {
                            GameObject orange_cities = Instantiate(orangeCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone4.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone4_full")
                        {
                            GameObject orange_cities = Instantiate(orangeCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone4_full.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone5")
                        {
                            GameObject yellow_cities = Instantiate(yellowCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone5.Add(vertex.transform.position);
                        }
                        if (city_zones[vertices[i]] == "zone5_full")
                        {
                            GameObject yellow_cities = Instantiate(yellowCities, vertices[i] + (z * vertexOffset) + new Vector3(0f, -0.7f, 0f), Quaternion.identity) as GameObject;
                            zone5_full.Add(vertex.transform.position);
                        }
                    }
                    definedPoints.Add(vertex.transform.position);
                }
            }
        }


        for (int h = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x < xSize; x++, h++)
            {
                horizontalLines[h] = new Vector3(x, 1f, z);
                if (delete_horizontalLines.Contains(horizontalLines[h]))
                {
                    continue;
                }
                else
                {
                    if (horizontal_bridges.ContainsKey(horizontalLines[h]))
                    {
                        if (horizontal_bridges[horizontalLines[h]] == "hbridge")
                        {
                            Vector3 node1 = new Vector3(x, 1f, z) + (z * vertexOffset);
                            Vector3 node2 = new Vector3(x + 1f, 1f, z) + (z * vertexOffset);
                            if (bridgeNodes.Contains(node1) == false)
                            {
                                bridgeNodes.Add(node1);
                            }
                            if (bridgeNodes.Contains(node2) == false)
                            {
                                bridgeNodes.Add(node2);
                            }
                            GameObject horizontal_bridge = Instantiate(hbridge, horizontalLines[h] + new Vector3(0.5f, 0f, 0f) + (z * vertexOffset), horizontalLine) as GameObject;
                            horizontal_bridge.gameObject.tag = "hBridge";
                        }
                    }
                    else
                    {
                        GameObject horizontal_line = Instantiate(line, horizontalLines[h] + new Vector3(0.5f, 0f, 0f) + (z * vertexOffset), horizontalLine) as GameObject;
                        horizontal_line.gameObject.tag = "hLine";
                    }
                }
            }
        }

        for (int v = 0, z = 0; z < zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, v++)
            {
                verticalLines[v] = new Vector3(x, 1f, z);
                if (delete_verticalLines.Contains(verticalLines[v]))
                {
                    continue;
                }
                else
                {
                    if (vertical_bridges.ContainsKey(verticalLines[v]))
                    {
                        if (vertical_bridges[verticalLines[v]] == "vbridge")
                        {
                            verticalLines[v] = verticalLineOffset + (z * vertexOffset) + new Vector3(x, 1f, z);
                            Vector3 node1 = new Vector3(x, 1f, z) + (z * vertexOffset);
                            Vector3 node2 = new Vector3(x, 1f, z) + (z * vertexOffset) + new Vector3(0.5f, 0f, 1f);
                            if (bridgeNodes.Contains(node1) == false)
                            {
                                bridgeNodes.Add(node1);
                            }
                            if (bridgeNodes.Contains(node2) == false)
                            {
                                bridgeNodes.Add(node2);
                            }
                            GameObject vertical_bridge = Instantiate(vbridge, verticalLines[v], verticalLine) as GameObject;
                            vertical_bridge.gameObject.tag = "vBridge";
                        }
                    }
                    else
                    {
                        verticalLines[v] = verticalLineOffset + (z * vertexOffset) + new Vector3(x, 1f, z);
                        GameObject vertical_line = Instantiate(vrline, verticalLines[v], verticalLine) as GameObject;
                        vertical_line.gameObject.tag = "vLine";
                    }
                }
            }
        }

        for (int d = 0, z = 0; z < zSize; z++)
        {
            for (int x = 1; x <= xSize; x++, d++)
            {
                diagonalLines[d] = new Vector3(x, 1f, z);
                if (delete_diagonalLines.Contains(diagonalLines[d]))
                {
                    continue;
                }
                else
                {
                    if (diagonal_bridges.ContainsKey(diagonalLines[d]))
                    {
                        if (diagonal_bridges[diagonalLines[d]] == "dbridge")
                        {
                            diagonalLines[d] = diagonalLineOffset + (z * vertexOffset) + new Vector3(x, 1f, z);
                            Vector3 node1 = new Vector3(x, 1f, z) + (z * vertexOffset);
                            Vector3 node2 = new Vector3(x, 1f, z) + (z * vertexOffset) + new Vector3(-0.5f, 0f, 1f);
                            if (bridgeNodes.Contains(node1) == false)
                            {
                                bridgeNodes.Add(node1);
                            }
                            if (bridgeNodes.Contains(node2) == false)
                            {
                                bridgeNodes.Add(node2);
                            }
                            GameObject diagonal_bridge = Instantiate(dbridge, diagonalLines[d], diagonalLine) as GameObject;
                            diagonal_bridge.gameObject.tag = "dBridge";
                        }
                    }
                    else
                    {
                        diagonalLines[d] = diagonalLineOffset + (z * vertexOffset) + new Vector3(x, 1f, z);
                        GameObject diagonal_line = Instantiate(drline, diagonalLines[d], diagonalLine) as GameObject;
                        diagonal_line.gameObject.tag = "dLine";
                    }
                }
            }
        }
    }

    // function for placing tracks
    void placetrack(int pturn)
    {
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.gameObject.tag == "dLine")
                {
                    float z = hit.collider.gameObject.transform.position.z;
                    float x = hit.collider.gameObject.transform.position.x;

                    Vector3 p1 = new Vector3(x + 0.25f, 1f, z - 0.5f);
                    Vector3 p2 = new Vector3(x - 0.25f, 1f, z + 0.5f);

                    Debug.Log(p1);
                    Debug.Log(p2);

                    if (playersCollections[pturn].Contains(p1) || playersCollections[0].Contains(p2))
                    {
                        if (playersCollections[pturn].Contains(p1))
                        {
                            playersCollections[pturn].Add(p2);
                        }
                        else
                        {
                            playersCollections[pturn].Add(p1);
                        }
                        Debug.Log("Players checked if connected");
                        GameObject block = Instantiate(TrackBlocks, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
                        layedTracks += 1;
                        UpdateCount();
                        try
                        {
                            addPlayersCollectionsIfConnected(0);
                        }
                        catch
                        {
                            return;
                        }
                    }
                    foreach (Vector3 p in playersCollections[pturn])
                    {
                        Debug.Log("player1sCollection points: " + p);
                    }

                }
                else if (hit.collider.gameObject.tag == "dBridge" && layedTracks == 0)
                {
                    float z = hit.collider.gameObject.transform.position.z;
                    float x = hit.collider.gameObject.transform.position.x;

                    Vector3 p1 = new Vector3(x + 0.25f, 1f, z - 0.5f);
                    Vector3 p2 = new Vector3(x - 0.25f, 1f, z + 0.5f);

                    Debug.Log(p1);
                    Debug.Log(p2);

                    if (playersCollections[pturn].Contains(p1) || playersCollections[0].Contains(p2))
                    {
                        if (playersCollections[pturn].Contains(p1))
                        {
                            playersCollections[pturn].Add(p2);
                        }
                        else
                        {
                            playersCollections[pturn].Add(p1);
                        }
                        Debug.Log("Players checked if connected");
                        GameObject block = Instantiate(TrackBlocks, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
                        layedTracks += 2;
                        UpdateCount();
                        try
                        {
                            addPlayersCollectionsIfConnected(0);
                        }
                        catch
                        {
                            return;
                        }
                    }
                    foreach (Vector3 p in playersCollections[pturn])
                    {
                        Debug.Log("player1sCollection points: " + p);
                    }

                }
                else if (hit.collider.gameObject.tag == "hBridge" && layedTracks == 0)
                {
                    float z = hit.collider.gameObject.transform.position.z;
                    float x = hit.collider.gameObject.transform.position.x;

                    Vector3 p1 = new Vector3(x + 0.5f, 1f, z);
                    Vector3 p2 = new Vector3(x - 0.5f, 1f, z);

                    Debug.Log(p1);
                    Debug.Log(p2);

                    if (playersCollections[pturn].Contains(p1) || playersCollections[0].Contains(p2))
                    {
                        if (playersCollections[pturn].Contains(p1))
                        {
                            playersCollections[pturn].Add(p2);
                        }
                        else
                        {
                            playersCollections[pturn].Add(p1);
                        }
                        Debug.Log("Players checked if connected");
                        GameObject block = Instantiate(TrackBlocks, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
                        layedTracks += 2;
                        UpdateCount();
                        try
                        {
                            addPlayersCollectionsIfConnected(0);
                        }
                        catch
                        {
                            return;
                        }
                    }
                    foreach (Vector3 p in playersCollections[pturn])
                    {
                        Debug.Log("player1sCollection points: " + p);
                    }
                }
                else if (hit.collider.gameObject.tag == "vBridge" && layedTracks == 0)
                {
                    float z = hit.collider.gameObject.transform.position.z;
                    float x = hit.collider.gameObject.transform.position.x;

                    Vector3 p1 = new Vector3(x - 0.25f, 1f, z - 0.5f);
                    Vector3 p2 = new Vector3(x + 0.25f, 1f, z + 0.5f);

                    Debug.Log(p1);
                    Debug.Log(p2);

                    if (playersCollections[pturn].Contains(p1) || playersCollections[0].Contains(p2))
                    {
                        if (playersCollections[pturn].Contains(p1))
                        {
                            playersCollections[pturn].Add(p2);
                        }
                        else
                        {
                            playersCollections[pturn].Add(p1);
                        }
                        Debug.Log("Players checked if connected");
                        GameObject block = Instantiate(TrackBlocks, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
                        layedTracks += 2;
                        UpdateCount();
                        try
                        {
                            addPlayersCollectionsIfConnected(0);
                        }
                        catch
                        {
                            return;
                        }
                    }
                    foreach (Vector3 p in playersCollections[pturn])
                    {
                        Debug.Log("player1sCollection points: " + p);
                    }
                }
                else if (hit.collider.gameObject.tag == "hLine")
                {
                    float z = hit.collider.gameObject.transform.position.z;
                    float x = hit.collider.gameObject.transform.position.x;

                    Vector3 p1 = new Vector3(x + 0.5f, 1f, z);
                    Vector3 p2 = new Vector3(x - 0.5f, 1f, z);

                    Debug.Log(p1);
                    Debug.Log(p2);


                    if (playersCollections[pturn].Contains(p1) || playersCollections[0].Contains(p2))
                    {
                        if (playersCollections[pturn].Contains(p1))
                        {
                            playersCollections[pturn].Add(p2);
                        }
                        else
                        {
                            playersCollections[pturn].Add(p1);
                        }
                        Debug.Log("Players checked if connected");
                        GameObject block = Instantiate(TrackBlocks, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
                        layedTracks += 1;
                        UpdateCount();
                        try
                        {
                            addPlayersCollectionsIfConnected(0);
                        }
                        catch
                        {
                            return;
                        }
                    }
                    foreach (Vector3 p in playersCollections[pturn])
                    {
                        Debug.Log("player1sCollection points: " + p);
                    }
                }
                else if (hit.collider.gameObject.tag == "vLine")
                {
                    float z = hit.collider.gameObject.transform.position.z;
                    float x = hit.collider.gameObject.transform.position.x;

                    Vector3 p1 = new Vector3(x - 0.25f, 1f, z - 0.5f);
                    Vector3 p2 = new Vector3(x + 0.25f, 1f, z + 0.5f);

                    Debug.Log(p1);
                    Debug.Log(p2);


                    if (playersCollections[pturn].Contains(p1) || playersCollections[0].Contains(p2))
                    {
                        if (playersCollections[pturn].Contains(p1))
                        {
                            playersCollections[pturn].Add(p2);
                        }
                        else
                        {
                            playersCollections[pturn].Add(p1);
                        }
                        Debug.Log("Players checked if connected");
                        GameObject block = Instantiate(TrackBlocks, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
                        layedTracks += 1;
                        UpdateCount();
                        try
                        {
                            addPlayersCollectionsIfConnected(0);
                        }
                        catch
                        {
                            return;
                        }
                    }
                    foreach (Vector3 p in playersCollections[pturn])
                    {
                        Debug.Log("player1sCollection points: " + p);
                    }
                }
            }
        }
    }

    //disables clicking on line until all the players have placed their hub
    void disableTrackInstantiation()
    {
        if (gameObject.tag == "hLine" || gameObject.tag == "vLine" || gameObject.tag == "dLine" || gameObject.tag == "hBridge" || gameObject.tag == "dBridge" || gameObject.tag == "vBridge")
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
    }


    //disables clicking on vertex as soon as all the players have place their hub
    void disableHubInstantiation()
    {
        if (gameObject.tag == "Point")
        {
            gameObject.GetComponent<SphereCollider>().enabled = false;
        }
    }


    //lays tracks
    void addTracks()
    {
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "hLine" || hit.collider.gameObject.tag == "vLine" || hit.collider.gameObject.tag == "dLine")
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject block = Instantiate(TrackBlocks, hit.collider.gameObject.transform.position + new Vector3(0.0f, 0.25f, 0.0f), hit.collider.gameObject.transform.rotation) as GameObject;
                    Debug.Log(hit.collider.gameObject.tag);
                }
            }
        }
    }


    //checks if all players have layed their hubs
    bool checkAllPlayersHaveHub()
    {
        if (maxHubs == hubCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public ArrayList bridgeNodeList()
    {
        return bridgeNodes;
    }

    // find shortest distances in the graph from a node
    public Dictionary<Vector3, int> shortest_distances(Graph G, Vector3 v)
    {
        Dictionary<Vector3, int> distance_so_far = new Dictionary<Vector3, int>();
        Dictionary<Vector3, int> final_distance = new Dictionary<Vector3, int>();
        //		Dictionary<Vector3, List<Vector3>> path_to = new Dictionary<Vector3, List<Vector3>> ();
        distance_so_far[v] = 0;
        //		path_to [v] = null;
        while (final_distance.Count < G.nodes.Count)
        {
            Vector3 w = shortest_distance_node(distance_so_far);
            final_distance[w] = distance_so_far[w];
            distance_so_far.Remove(w);
            foreach (Vector3 x in G.get_neighbour_nodes(w))
            {
                if (final_distance.ContainsKey(x) == false)
                {
                    if (distance_so_far.ContainsKey(x) == false)
                    {
                        distance_so_far[x] = final_distance[w] + G.get_distance_between_nodes(x, w);
                        //						path_to [x] = path_to [w];
                        //						path_to [x].Add (w);
                    }
                    else if (final_distance[w] + G.get_distance_between_nodes(x, w) < distance_so_far[x])
                    {
                        distance_so_far[x] = final_distance[w] + G.get_distance_between_nodes(x, w);
                        //						path_to [x] = path_to [w];
                        //						path_to [x].Add (w);
                    }
                }
            }
        }
        Debug.Log("final_distance.Count: " + final_distance.Count);
        Debug.Log("G.nodes.Count: " + G.nodes.Count);

        //		Debug.Log ("path_to: " + path_to.Count);

        return final_distance;

    }

    public Vector3 shortest_distance_node(Dictionary<Vector3, int> d)
    {
        Vector3 best_node = new Vector3();
        int best_value = 100000;
        foreach (Vector3 x in d.Keys)
        {
            if (d[x] < best_value)
            {
                best_node = x;
                best_value = d[x];
            }
        }
        return best_node;
    }


    // find the path to the victory points from a node
    public Dictionary<Vector3, List<Vector3>> shortest_path(Graph G, Vector3 v)
    {
        Dictionary<Vector3, List<Vector3>> path_to_dest = new Dictionary<Vector3, List<Vector3>>();
        Dictionary<Vector3, int> distance_so_far = new Dictionary<Vector3, int>();
        Dictionary<Vector3, int> final_distance = new Dictionary<Vector3, int>();
        Dictionary<Vector3, Vector3> path_to = new Dictionary<Vector3, Vector3>();
        Dictionary<Vector3, List<Vector3>> paths_to_vps = new Dictionary<Vector3, List<Vector3>>();
        distance_so_far[v] = 0;
        path_to[v] = v;
        while (final_distance.Count < G.nodes.Count)
        {
            Vector3 w = shortest_distance_node(distance_so_far);
            final_distance[w] = distance_so_far[w];
            distance_so_far.Remove(w);
            foreach (Vector3 x in G.get_neighbour_nodes(w))
            {
                if (final_distance.ContainsKey(x) == false)
                {
                    if (distance_so_far.ContainsKey(x) == false)
                    {
                        distance_so_far[x] = final_distance[w] + G.get_distance_between_nodes(x, w);
                        path_to[x] = w;
                    }
                    else if (final_distance[w] + G.get_distance_between_nodes(x, w) < distance_so_far[x])
                    {
                        distance_so_far[x] = final_distance[w] + G.get_distance_between_nodes(x, w);
                        path_to[x] = w;
                    }
                }
            }
        }

        foreach (Vector3 point in botsvp)
        {
            if (path_to.ContainsKey(point))
            {
                if (path_to[point] != v)
                {
                    bool source_point_found = false;
                    Vector3 pointer_point = point;
                    paths_to_vps[point] = new List<Vector3>();
                    paths_to_vps[point].Add(point);
                    while (source_point_found != true)
                    {
                        paths_to_vps[point].Add(path_to[pointer_point]);
                        pointer_point = path_to[pointer_point];
                        if (pointer_point == v)
                        {
                            source_point_found = true;
                            paths_to_vps[point].Remove(v);
                        }
                    }
                }
                else
                {
                    if (point != (Vector3)playersCollections[1][0])
                    {
                        paths_to_vps[point] = new List<Vector3>();
                        paths_to_vps[point].Add(point);
                    }
                }
            }
        }

        Vector3 shortest_dest = get_shortest_dest(paths_to_vps);
        path_to_dest[shortest_dest] = paths_to_vps[shortest_dest];

        Debug.Log("path_to: " + path_to.Count);

        return path_to_dest;
    }

    //public Dictionary<Vector3, List<Vector3>> shortest_path (Graph G, Vector3 v, Vector3 dest){
    //	Dictionary<Vector3, int> distance_so_far = new Dictionary<Vector3, int> ();
    //	Dictionary<Vector3, int> final_distance = new Dictionary<Vector3, int> ();
    //	Dictionary<Vector3, Vector3> path_to = new Dictionary<Vector3, Vector3> ();
    //	Dictionary<Vector3, List<Vector3>> path_to_dest = new Dictionary<Vector3, List<Vector3>> ();
    //	distance_so_far [v] = 0;
    //	path_to [v] = v;
    //	while (final_distance.Count < G.nodes.Count) {
    //		Vector3 w = shortest_distance_node (distance_so_far);
    //		final_distance [w] = distance_so_far [w];
    //		distance_so_far.Remove (w);
    //		foreach (Vector3 x in G.get_neighbour_nodes(w)) {
    //			if (final_distance.ContainsKey (x) == false) {
    //				if (distance_so_far.ContainsKey (x) == false) {
    //					distance_so_far [x] = final_distance [w] + G.get_distance_between_nodes (x, w);
    //					path_to [x] = w;
    //				} else if (final_distance [w] + G.get_distance_between_nodes (x, w) < distance_so_far [x]) {
    //					distance_so_far [x] = final_distance [w] + G.get_distance_between_nodes (x, w);
    //					path_to [x] = w;
    //				}
    //			}
    //			if (final_distance.ContainsKey (dest)) {
    //				break;
    //			}
    //		}
    //	}

    //	if (path_to.ContainsKey (dest)) {
    //		if (path_to [dest] != v) {
    //			bool source_point_found = false;
    //			Vector3 pointer_point = dest;
    //			path_to_dest [dest] = new List<Vector3> ();
    //			path_to_dest [dest].Add (dest);
    //			while (source_point_found != true) {
    //				path_to_dest [dest].Add (path_to [pointer_point]);
    //				pointer_point = path_to [pointer_point];
    //				if (pointer_point == v) {
    //					source_point_found = true;
    //				}
    //			}
    //		}
    //	}


    //	Debug.Log ("path_to: " + path_to.Count);

    //	return path_to_dest;
    //}

    // gets the point from which the bot has to start laying tracks again
    public Vector3 get_current_point()
    {
        Dictionary<Vector3, Vector3> point_source = new Dictionary<Vector3, Vector3>();
        Dictionary<Vector3, int> shortest_distances_map = new Dictionary<Vector3, int>();
        foreach (Vector3 point in playersCollections[1])
        {
            Dictionary<Vector3, int> d_to_vps = shortest_distances(graph, point);
            for (int i = 0; i < botsvp.Count; i++)
            {
                if (shortest_distances_map.ContainsKey((Vector3)botsvp[i]) == false)
                {
                    shortest_distances_map[(Vector3)botsvp[i]] = d_to_vps[(Vector3)botsvp[i]];
                    point_source[(Vector3)botsvp[i]] = point;
                }
                else
                {
                    if (shortest_distances_map[(Vector3)botsvp[i]] > d_to_vps[(Vector3)botsvp[i]])
                    {
                        shortest_distances_map[(Vector3)botsvp[i]] = d_to_vps[(Vector3)botsvp[i]];
                        point_source[(Vector3)botsvp[i]] = point;
                    }
                }
            }
        }
        Vector3 shortest_point_to_goto = shortest_distance_node(shortest_distances_map);
        Debug.Log("shortest node to goto from the current_point " + shortest_point_to_goto);
        return point_source[shortest_point_to_goto];
    }

    public Vector3 get_shortest_dest(Dictionary<Vector3, List<Vector3>> dict)
    {
        Dictionary<Vector3, int> distances = new Dictionary<Vector3, int>();
        foreach (Vector3 x in dict.Keys)
        {
            distances[x] = dict[x].Count;
        }
        Vector3 shortest_node = shortest_distance_node(distances);
        return shortest_node;
    }


    // checks if the player is connected to other players
    public void addPlayersCollectionsIfConnected(int player)
    {
        //		Debug.Log ("Connection Function Called");
        //		Debug.Log ("Max hubs: " + maxHubs);
        //		Debug.Log ("Player: " + player);
        for (int i = 0; i < maxHubs; i++)
        {
            Debug.Log("Checking for player inside loop " + i);
            if (i == player)
            {
                continue;
            }
            else
            {
                foreach (Vector3 x in playersCollections[player])
                {
                    //					Debug.Log ("Checking if any players are connected");
                    if (playersCollections[i].Contains(x))
                    {
                        //						Debug.Log ("Player " + player + " is connected to " + i);
                        //						for (int count = 0; count < playersCollections[player].Count; count++) {
                        foreach (Vector3 point in playersCollections[player])
                        {
                            //							Debug.Log ("Points are added to " + player);
                            if (playersCollections[i].Contains(point) == false)
                            {
                                playersCollections[i].Add(point);
                            }
                        }
                        //						for(int count = 0; count < playersCollections[i].Count; count++) {
                        foreach (Vector3 point in playersCollections[i])
                        {
                            //							Debug.Log ("Points are added to " + i);
                            if (playersCollections[player].Contains(point) == false)
                            {
                                playersCollections[player].Add(point);
                            }
                        }
                    }
                    //					else {
                    //						Debug.Log ("No connection found for " + i);
                    //					}
                }
            }
        }
    }

}