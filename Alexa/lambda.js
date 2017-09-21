'use strict';

/**
 * This sample demonstrates a simple skill built with the Amazon Alexa Skills Kit.
 * The Intent Schema, Custom Slots, and Sample Utterances for this skill, as well as
 * testing instructions are located at http://amzn.to/1LzFrj6
 *
 * For additional samples, visit the Alexa Skills Kit Getting Started guide at
 * http://amzn.to/1LGWsLG
 */


// --------------- Helpers that build all of the responses -----------------------

var queueURL = "https://sqs.us-east-1.amazonaws.com/755552506636/BridgeInvasionQueue.fifo"
var AWS = require('aws-sdk');
var sqs = new AWS.SQS({region : 'us-east-1'});
var BOT_NAMES = ['david', 'wiley'];
var TARGET_NAMES = ['mike' , 'wallace', 'barbara', 'jessica', 'phillip', 'jeff'];
var ADVANCE_SPEECH = ['Affirmative', 'I am on it', 'Got it']
var FIRE_SPEECH = ['Target confirmed ', 'Firing at ', 'Consider it done']
var TAKE_COVER_SPEECH = ['Falling back', 'I\'m retreating', 'I\'m out of here']

function buildSpeechletResponse(title, output, repromptText, shouldEndSession) {
    return {
        outputSpeech: {
            type: 'PlainText',
            text: output,
        },
        card: {
            type: 'Simple',
            title: `SessionSpeechlet - ${title}`,
            content: `SessionSpeechlet - ${output}`,
        },
        shouldEndSession,
    };
}

function buildResponse(sessionAttributes, speechletResponse) {
    return {
        version: '1.0',
        sessionAttributes,
        response: speechletResponse,
    };
}


// --------------- Functions that control the skill's behavior -----------------------

function getWelcomeResponse(callback) {
    // If we wanted to initialize the session to have some attributes we could add those here.
    const sessionAttributes = {};
    const cardTitle = 'Welcome';
    const speechOutput = 'Welcome to Bridge Invasion';
    // If the user either does not reply to the welcome message or says something that is not
    // understood, they will be prompted again with this text.
    const repromptText = 'I did not understand. Please try again.';
    const shouldEndSession = false;

    callback(sessionAttributes,
        buildSpeechletResponse(cardTitle, speechOutput, repromptText, shouldEndSession));
}

function handleSessionEndRequest(callback) {
    const cardTitle = 'Session Ended';
    const speechOutput = 'Thank you for playing Bridge Invasion';
    // Setting this to true ends the session and exits the skill.
    const shouldEndSession = true;

    callback({}, buildSpeechletResponse(cardTitle, speechOutput, null, shouldEndSession));
}

function sendSQSMessage(message) {
    var ts = Math.round((new Date()).getTime() / 1000);

    var params = {
        MessageBody: message,
        MessageGroupId: "1",
        MessageDeduplicationId: ts.toString(),
        QueueUrl: queueURL,
        DelaySeconds: 0
    };

    sqs.sendMessage(params, function(err, data) {
        if (err) {
            console.log(err);
        } else {
            console.log(data);
        }
    });
}

function getRandomInt(min, max) {
  min = Math.ceil(min);
  max = Math.floor(max);
  return Math.floor(Math.random() * (max - min)) + min; //The maximum is exclusive and the minimum is inclusive
}

/**
 * Sets the color in the session and prepares the speech to reply to the user.
 */
function advance(intent, session, callback) {
    const cardTitle = intent.name;
    const botNameSlot = intent.slots.BotName;
    let repromptText = '';
    let sessionAttributes = {};
    const shouldEndSession = false;
    let speechOutput = '';

    if (botNameSlot) {
        const botName = botNameSlot.value;
        if (BOT_NAMES.indexOf(botName.toLowerCase()) > -1) {
            speechOutput = ADVANCE_SPEECH[getRandomInt(0 , 3)];
            repromptText = "Test Reprompt";
            sendSQSMessage(botName + " advance");
        }
        else {
            speechOutput = 'Who is ' + botName;
        }
    }
    else {
        speechOutput = 'Please use an American name';
    }
    callback(sessionAttributes,
        buildSpeechletResponse(cardTitle, speechOutput, repromptText, shouldEndSession));
}

function fire(intent, session, callback) {
    const cardTitle = intent.name;
    const botNameSlot = intent.slots.BotName;
    const targetSlot = intent.slots.Target;
    let repromptText = '';
    let sessionAttributes = {};
    const shouldEndSession = false;
    let speechOutput = '';

    if (botNameSlot && targetSlot) {
        const botName = botNameSlot.value;
        const target = targetSlot.value;
        if (BOT_NAMES.indexOf(botName.toLowerCase()) > -1 && TARGET_NAMES.indexOf(target.toLowerCase()) > -1) {
            var randInt = getRandomInt(0, 3);
            speechOutput = FIRE_SPEECH[randInt];

            if (randInt != 2) {
                speechOutput += target
            }

            repromptText = "Test Reprompt";

            sendSQSMessage(botName + " fire " + target);
        } 

        else {
            speechOutput = 'Who are these people?';
        }
    } 
    else {
        speechOutput = 'Please use American names';
    }
    callback(sessionAttributes,
        buildSpeechletResponse(cardTitle, speechOutput, repromptText, shouldEndSession));
}

function takeCover(intent, session, callback) {
    const cardTitle = intent.name;
    const botNameSlot = intent.slots.BotName;
    let repromptText = '';
    let sessionAttributes = {};
    const shouldEndSession = false;
    let speechOutput = '';

    if (botNameSlot) {
        const botName = botNameSlot.value;
        if (BOT_NAMES.indexOf(botName.toLowerCase()) > -1) {
            speechOutput = TAKE_COVER_SPEECH[getRandomInt(0, 3)];
            repromptText = "Test Reprompt";

            sendSQSMessage(botName + " takeCover");
        }
        else {
            speechOutput = 'Who is ' + botName;
        }
    }
    else {
        speechOutput = 'Please use an American name';
    }
    callback(sessionAttributes,
        buildSpeechletResponse(cardTitle, speechOutput, repromptText, shouldEndSession));
}


// --------------- Events -----------------------

/**
 * Called when the session starts.
 */
function onSessionStarted(sessionStartedRequest, session) {
    console.log(`onSessionStarted requestId=${sessionStartedRequest.requestId}, sessionId=${session.sessionId}`);
}

/**
 * Called when the user launches the skill without specifying what they want.
 */
function onLaunch(launchRequest, session, callback) {
    console.log(`onLaunch requestId=${launchRequest.requestId}, sessionId=${session.sessionId}`);

    // Dispatch to your skill's launch.
    getWelcomeResponse(callback);
}

/**
 * Called when the user specifies an intent for this skill.
 */
function onIntent(intentRequest, session, callback) {
    console.log(`onIntent requestId=${intentRequest.requestId}, sessionId=${session.sessionId}`);

    const intent = intentRequest.intent;
    const intentName = intentRequest.intent.name;

    // Dispatch to your skill's intent handlers
    if (intentName === 'AdvanceIntent') {
        advance(intent, session, callback);
    } else if (intentName === 'FireIntent') {
        fire(intent, session, callback);
    } else if (intentName === 'TakeCoverIntent') {
        takeCover(intent, session, callback);
    } else if (intentName === 'AMAZON.HelpIntent') {
        getWelcomeResponse(callback);
    } else if (intentName === 'AMAZON.StopIntent' || intentName === 'AMAZON.CancelIntent') {
        handleSessionEndRequest(callback);
    } else {
        throw new Error('Invalid intent');
    }
}

/**
 * Called when the user ends the session.
 * Is not called when the skill returns shouldEndSession=true.
 */
function onSessionEnded(sessionEndedRequest, session) {
    console.log(`onSessionEnded requestId=${sessionEndedRequest.requestId}, sessionId=${session.sessionId}`);
    // Add cleanup logic here
}


// --------------- Main handler -----------------------

// Route the incoming request based on type (LaunchRequest, IntentRequest,
// etc.) The JSON body of the request is provided in the event parameter.
exports.handler = (event, context, callback) => {
    try {
        console.log(`event.session.application.applicationId=${event.session.application.applicationId}`);

        /**
         * Uncomment this if statement and populate with your skill's application ID to
         * prevent someone else from configuring a skill that sends requests to this function.
         */
        /*
        if (event.session.application.applicationId !== 'amzn1.echo-sdk-ams.app.[unique-value-here]') {
             callback('Invalid Application ID');
        }
        */

        if (event.session.new) {
            onSessionStarted({ requestId: event.request.requestId }, event.session);
        }

        if (event.request.type === 'LaunchRequest') {
            onLaunch(event.request,
                event.session,
                (sessionAttributes, speechletResponse) => {
                    callback(null, buildResponse(sessionAttributes, speechletResponse));
                });
        } else if (event.request.type === 'IntentRequest') {
            onIntent(event.request,
                event.session,
                (sessionAttributes, speechletResponse) => {
                    callback(null, buildResponse(sessionAttributes, speechletResponse));
                });
        } else if (event.request.type === 'SessionEndedRequest') {
            onSessionEnded(event.request, event.session);
            callback();
        }
    } catch (err) {
        callback(err);
    }
};
