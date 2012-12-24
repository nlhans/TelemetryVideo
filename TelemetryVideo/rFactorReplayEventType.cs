namespace TelemetryVideo
{
    public enum rFactorReplayEventType
    {
        ETYPE_NOTYPE = 63, // doesn’t need to be anything in particular, but this is

        // ECLASS_LOC events
        ETYPE_CONE = 0, // cone state
        ETYPE_POST = 1, // post state
        ETYPE_SIGN = 2, // sign state
        ETYPE_WHEEL = 3, // wheel state
        ETYPE_WING = 4, // wing state
        ETYPE_PART = 5, // part state
        // note: do not change order of the ETYPE_*LOC events!
        ETYPE_ZIPUPDATE = 6, // tells server that last zip can be safely extrapolated
        ETYPE_ZIPLOC_R = 7, // compressed vehicle loc (more detailed than SIMPLELOC)
        ETYPE_ZIPLOC_N = 8, // compressed vehicle loc
        ETYPE_ZIPLOC_1 = 9, // compressed vehicle loc
        ETYPE_ZIPLOC_2 = 10, // compressed vehicle loc
        ETYPE_ZIPLOC_3 = 11, // compressed vehicle loc
        ETYPE_ZIPLOC_4 = 12, // compressed vehicle loc
        ETYPE_ZIPLOC_5 = 13, // compressed vehicle loc
        ETYPE_ZIPLOC_6 = 14, // compressed vehicle loc
        ETYPE_ZIPLOC_7 = 15, // compressed vehicle loc
        ETYPE_SIMPLELOC_00 = 16, // simplified location event (smaller than ZIPLOC)
        ETYPE_SIMPLELOC_01 = 17, // simplified location event
        ETYPE_SIMPLELOC_02 = 18, // simplified location event
        ETYPE_SIMPLELOC_03 = 19, // simplified location event
        ETYPE_SIMPLELOC_04 = 20, // simplified location event
        ETYPE_SIMPLELOC_05 = 21, // simplified location event
        ETYPE_SIMPLELOC_06 = 22, // simplified location event
        ETYPE_SIMPLELOC_07 = 23, // simplified location event
        ETYPE_SIMPLELOC_08 = 24, // simplified location event
        ETYPE_SIMPLELOC_09 = 25, // simplified location event
        ETYPE_SIMPLELOC_10 = 26, // simplified location event
        ETYPE_SIMPLELOC_11 = 27, // simplified location event
        ETYPE_SIMPLELOC_12 = 28, // simplified location event
        ETYPE_SIMPLELOC_13 = 29, // simplified location event
        ETYPE_SIMPLELOC_14 = 30, // simplified location event
        ETYPE_SIMPLELOC_15 = 31, // simplified location event
        ETYPE_SIMPLELOC_16 = 32, // simplified location event
        ETYPE_SIMPLELOC_17 = 33, // simplified location event
        ETYPE_SIMPLELOC_18 = 34, // simplified location event
        ETYPE_SIMPLELOC_19 = 35, // simplified location event
        ETYPE_SIMPLELOC_20 = 36, // simplified location event
        ETYPE_SIMPLELOC_21 = 37, // simplified location event
        ETYPE_SIMPLELOC_22 = 38, // simplified location event
        ETYPE_SIMPLELOC_23 = 39, // simplified location event
        ETYPE_SIMPLELOC_24 = 40, // simplified location event
        ETYPE_SIMPLELOC_25 = 41, // simplified location event
        ETYPE_SIMPLELOC_26 = 42, // simplified location event
        ETYPE_SIMPLELOC_27 = 43, // simplified location event
        ETYPE_SIMPLELOC_28 = 44, // simplified location event
        ETYPE_SIMPLELOC_29 = 45, // simplified location event
        ETYPE_SIMPLELOC_30 = 46, // simplified location event
        ETYPE_SIMPLELOC_31 = 47, // simplified location event
        ETYPE_SIMPLELOC_32 = 48, // simplified location event

        // ECLASS_VFX events
        ETYPE_BACKFIRE = 0, // veh backfire animation
        ETYPE_LIGHT = 1, // veh rain light switch
        ETYPE_TERRAIN = 2, // terrain conditions event
        ETYPE_COPTER = 3, // sends the helicopter on its way
        ETYPE_WEATHER = 4, // weather updates from the server
        ETYPE_CLOUDMAP = 5, // cloud texture map names
        ETYPE_LIGHTNING = 6, // lightning
        ETYPE_DENT_BODY = 7, // dent bodywork
        ETYPE_RESET_BODY = 8, // reset bodywork (vehicle reset to pits or something)
        ETYPE_WALLSKID = 9, // generate skid on wall
        ETYPE_RACELIGHTS = 10, // start lights, pit entrance & exit lights, maybe caution lights around track?

        // ECLASS_SFX events
        ETYPE_VEHSFX_START = 0, // starts a vehicle sound effect
        ETYPE_THUNDER = 1, // thunder
        ETYPE_PIT_HORN = 2, // pit horn plays when someone enters pits

        ETYPE_SYNC_NOTIFY = 0, // server sends this to client if client should send a sync request (state change, etc)
        ETYPE_PLAYER_READY = 1, // message from client to server requesting verification
        ETYPE_PLAYER_VERIFY = 2, // server to client, data verify along with any changes
        ETYPE_TRACKTOLOAD = 3, // server to client, new track is being loaded
        ETYPE_CHATMSG = 4, // chat message, bi-directional
        ETYPE_SCOREBOARD = 5, // score update
        ETYPE_CHECKPOINT = 6,
        ETYPE_SPEEDCOMP = 7,
        ETYPE_NEWSTATIONS = 8, // complete new list of all active flag stations
        ETYPE_UPDATESTATIONS = 9, // update flag stations

        ETYPE_COUNTDOWN = 10, // start countdown
        ETYPE_NEXTPHASE = 11, // move to next phase
        ETYPE_SKIPFORMATION = 12, // skip formation lap

        ETYPE_YELLOW_FLAG = 13,
        ETYPE_STOP_GO = 14, // received stop/go
        ETYPE_SERVE_STOP_GO = 15, // served stop/go

        ETYPE_DNF = 16, // DNF player
        ETYPE_UNDO_DNF = 17, // Undo DNF
        ETYPE_DISQUALIFY = 18,
        ETYPE_CLIENTKICK = 19, // used to kick client

        ETYPE_PIT_BLUE_ON = 20, // pit exit flashing blue
        ETYPE_PIT_BLUE_OFF = 21, // pit exit not flashing blue

        ETYPE_TRACK_ORDER = 24, // order of vehicles on track (while under yellow flag)
        ETYPE_SAFETY_CAR_ON = 25, // safety car is active
        ETYPE_SAFETY_CAR_OFF = 26, // safety car is not active
        ETYPE_SAFETY_CAR_LAPS = 27, // set the number of safety car formation laps

        ETYPE_NEW_ENGINE = 28, // slot needed new engine during warmup (so send to back of grid)
        ETYPE_QUAL_ORDER = 29, // listing of new qualifying order (used when player

        ETYPE_TORDER_ADD = 30, // client -> server msg, if client is added from track order
        ETYPE_TORDER_REMOVE = 31, // client -> server msg, if client is removed from track order
        ETYPE_TORDER_TOBACK = 32, // client -> server msg, if client is removed from track order
        ETYPE_TRACK_CLOSED = 33, // server -> client msg, track has been closed for business
        ETYPE_TRACK_OPENED = 34, // server -> client msg, track has been opened for business
        ETYPE_ENTRY_DATA = 35, // client -> server, client is sending entry data
        ETYPE_ENTRY_DATA_OK = 36, // server -> client, entry data is ok
        ETYPE_ENTRY_DATA_NO = 37, // server -> client, entry data not allowed

        ETYPE_DRIVER_CHANGE = 38, // sent from previous driver to new driver (through server if necessary)
        ETYPE_DRIVER_CHANGE_NOTIFY = 39, // sent from previous driver to new driver (through server if necessary)
        ETYPE_PASSENGER_CHANGE = 40, // notifies drivers that a passenger has joined/left their vehicle
        ETYPE_FILE_REQUEST = 41,
        ETYPE_FILE_HEADER = 42,
        ETYPE_NET_AI_INFO = 43, // server info about one multiplayer AI
        ETYPE_NET_AI_REQUEST = 44, // request server info about one or more AIs
        ETYPE_NET_RESTART = 45, // restart event or race in multiplayer
        ETYPE_CLIENT_VIEW = 46, // what client is looking at (slot or actual position) for server throttling purposes

        ETYPE_VOICE_CHAT_MIXED = 47, // voice chat data thats been mixed by the server and ready for client consumption
        ETYPE_VOICE_CHAT_CLIENT = 48, // voice chat data that the clients are submitting to the server

        // ECLASS_OTHER events
        ETYPE_FIX = 0, // fix damage
        ETYPE_PITSTATE = 1, // change in pitstate
        ETYPE_BASICSCORE = 2, // basic score
        ETYPE_BASICSCORECLIENT = 3, // basic score client
        ETYPE_SIMULATEINTERNET = 4, // simulate some attributes of the Internet for network testing purposes
        ETYPE_CALLVOTE_NEXTSESSION = 5, // “/callvote nextsession”
        ETYPE_CALLVOTE_NEXTEVENT = 6, // “/callvote nextrace”
        ETYPE_CALLVOTE_ADDAI = 7, // “/callvote addai”
        ETYPE_CALLVOTE_ADD5AI = 8, // “/callvote add5ai”
        ETYPE_CALLVOTE_REMOTESERVER = 9, // “/callvote kingme”
        ETYPE_CALLVOTE_RESTARTEVENT = 10, // “/callvote restartweekend”
        ETYPE_CALLVOTE_RESTARTRACE = 11, // “/callvote restartrace”
        ETYPE_VOTE_YES = 12, // “/vote yes”
        ETYPE_VOTE_NO = 13, // “/vote no”
        ETYPE_VOTEIGNORED_PARTICIPANT = 14, // “Vote ignored: Only active participants may vote”
        ETYPE_VOTEIGNORED_NOVOTE = 15, // “Vote ignored: There is no vote in progress”
        ETYPE_VOTEIGNORED_ONEVOTE = 16, // “Vote ignored: You can only vote once per issue”
        ETYPE_NODE_VOTED_YES = 17, // “%s voted YES to %s:” plus either “%d more to pass” or “vote PASSED”
        ETYPE_NODE_VOTED_NO = 18, // “%s voted NO to %s”
        ETYPE_CANNOTCALL_DISABLED = 19, // “Cannot call vote: voting for %s is disabled”
        ETYPE_CANNOTCALL_INPROGRESS = 20, // “Cannot call vote: there is a vote in progress”
        ETYPE_CANNOTCALL_JUSTENDED = 21, // “Cannot call vote: voted just ended (wait %d seconds)”
        ETYPE_CANNOTCALL_PARTICIPANT = 22, // “Cannot call vote: must be server or active participant”
        ETYPE_CANNOTCALL_BEFORERACE = 23, // “Cannot call vote: cannot restart race before race starts”
        ETYPE_CANNOTCALL_PASTRACE = 24, // “Cannot call vote: cannot advance session past race”
        ETYPE_CANNOTCALL_MINVOTERS = 25, // “Cannot call vote: %d voters required”
        ETYPE_CANNOTCALL_RACERESTARTS = 26, // “Cannot call vote: no more race restarts allowed”
        ETYPE_CANNOTCALL_MOREAI = 27, // “Cannot call vote: no more AIs can be added”
        ETYPE_VOTE_PROPOSED = 28, // “\”%s\” proposed by %s:” plus either “%d vote(s) needed to pass” or “vote PASSED”
        ETYPE_WITHHOLDING_GREEN = 29, // WITHHOLDING_GREEN_MSG defined in steward.hpp
        ETYPE_CANNOTCALL_PLRNOTFOUND = 31, // “Cannot call vote: player specified not found”
        ETYPE_ADMIN_MSG = 32, // Some admin message
        ETYPE_CANNOTCALL_SPECEVENT = 33, // “Cannot call vote: circuit not found or request disallowed”
    }
}