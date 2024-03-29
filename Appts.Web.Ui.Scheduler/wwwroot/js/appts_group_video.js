﻿
// simple JS interface for Agora.io web SDK

// app / channel settings
var agoraAppId = "7170f3361f744a148eff7d2966734c05"; // Set your Agora App ID
//var channelName = 'ao4';
var channelName = $('#apptschannelid').val();
var temptoken = $('#apptstoken').val();

// video profile settings
var cameraVideoProfile = '480p_4'; // 640 × 480 @ 30fps  & 750kbs
var screenVideoProfile = '480p_2'; // 640 × 480 @ 30fps

// create client instances for camera (client) and screen share (screenClient)
var client = AgoraRTC.createClient({ mode: 'rtc', codec: "h264" }); // h264 better detail at a higher motion
var screenClient = AgoraRTC.createClient({ mode: 'rtc', codec: 'vp8' }); // use the vp8 for better detail in low motion

// stream references (keep track of active streams) 
var remoteStreams = {}; // remote streams obj struct [id : stream] 

var localStreams = {
  camera: {
    id: "",
    stream: {}
  },
  screen: {
    id: "",
    stream: {}
  }
};

var mainStreamId; // reference to main stream
var screenShareActive = false; // flag for screen share 

// init Agora SDK
client.init(agoraAppId, function () {
  console.log("AgoraRTC client initialized");
  joinChannel(); // join channel upon successfull init
}, function (err) {
  console.log("[ERROR] : AgoraRTC client init failed", err);
});

client.on('stream-published', function (evt) {
  console.log("Publish local stream successfully");
});

// connect remote streams
client.on('stream-added', function (evt) {
  var stream = evt.stream;
  var streamId = stream.getId();
  console.log("new stream added: " + streamId);
  // Check if the stream is local
  if (streamId != localStreams.screen.id) {
    console.log('subscribe to remote stream:' + streamId);
    // Subscribe to the stream.
    client.subscribe(stream, function (err) {
      console.log("[ERROR] : subscribe stream failed", err);
    });
  }
});

client.on('stream-subscribed', function (evt) {
  var remoteStream = evt.stream;
  var remoteId = remoteStream.getId();
  remoteStreams[remoteId] = remoteStream;
  console.log("Subscribe remote stream successfully: " + remoteId);
  //if ($('#full-screen-video').is(':empty')) {
  //  mainStreamId = remoteId;
  //  remoteStream.play('full-screen-video');
  //} else {
  //  addRemoteStreamMiniView(remoteStream);
  //}

  addRemoteStreamMiniView(remoteStream);
});

// remove the remote-container when a user leaves the channel
client.on("peer-leave", function (evt) {
  var streamId = evt.stream.getId(); // the the stream id
  if (remoteStreams[streamId] != undefined) {
    remoteStreams[streamId].stop(); // stop playing the feed
    delete remoteStreams[streamId]; // remove stream from list
    if (streamId == mainStreamId) {
      var streamIds = Object.keys(remoteStreams);
      var randomId = streamIds[Math.floor(Math.random() * streamIds.length)]; // select from the remaining streams
      remoteStreams[randomId].stop(); // stop the stream's existing playback
      var remoteContainerID = '#' + randomId + '_container';
      $(remoteContainerID).empty().remove(); // remove the stream's miniView container
      remoteStreams[randomId].play('full-screen-video'); // play the random stream as the main stream
      mainStreamId = randomId; // set the new main remote stream
    } else {
      var remoteContainerID = '#' + streamId + '_container';
      $(remoteContainerID).empty().remove(); // 
    }
  }
});

// show mute icon whenever a remote has muted their mic
client.on("mute-audio", function (evt) {
  toggleVisibility('#' + evt.uid + '_mute', true);
  console.log('mute-audo event run')
});

client.on("unmute-audio", function (evt) {
  toggleVisibility('#' + evt.uid + '_mute', false);
});

// show user icon whenever a remote has disabled their video
client.on("mute-video", function (evt) {
  var remoteId = evt.uid;
  // if the main user stops their video select a random user from the list
  if (remoteId != mainStreamId) {
    // if not the main vidiel then show the user icon
    toggleVisibility('#' + remoteId + '_no-video', true);
  }
});

client.on("unmute-video", function (evt) {
  toggleVisibility('#' + evt.uid + '_no-video', false);
});


function getDevices(next) {
  AgoraRTC.getDevices(function (items) {
    console.log('items from getDevices ', items);
    items.filter(function (item) {
      return ["audioinput", "videoinput"].indexOf(item.kind) !== -1
    })
      .map(function (item) {
        return {
          name: item.label,
          value: item.deviceId,
          kind: item.kind,
        }
      })
    var videos = []
    var audios = []
    for (var i = 0; i < items.length; i++) {
      var item = items[i]
      if ("videoinput" == item.kind) {
        var name = item.label
        var value = item.deviceId
        if (!name) {
          name = "camera-" + videos.length
        }
        videos.push({
          name: name,
          value: value,
          kind: item.kind
        })
      }
      if ("audioinput" == item.kind) {
        var name = item.label
        var value = item.deviceId
        if (!name) {
          name = "microphone-" + audios.length
        }
        audios.push({
          name: name,
          value: value,
          kind: item.kind
        })
      }
    }
    next({ videos: videos, audios: audios })
  })
}


// join a channel
function joinChannel() {
  //var token = generateToken();
  //var userID = null; // set to null to auto generate uid on successfull connection

  //todo: get this info from hidden fields set-up during rendering
  var token = $('#apptstoken').val()
  var userID = $('#apptsuserid').val();

  client.join(token, channelName, userID, function (uid) {
    console.log("User " + uid + " join channel successfully");
    createCameraStream(uid);
    localStreams.camera.id = uid; // keep track of the stream uid 
  }, function (err) {
    console.log("[ERROR] : join channel failed", err);
  });
}

// video streams for channel
function createCameraStream(uid) {
  var localStream = AgoraRTC.createStream({
    streamID: uid,
    audio: true,
    video: true,
    screen: false
  });
  localStream.setVideoProfile(cameraVideoProfile);

  ////////working for screenshare
  //////var localStream = AgoraRTC.createStream({
  //////  streamID: uid,
  //////  audio: true,
  //////  //video: true,
  //////  video: false,
  //////  screen: false
  //////});
  ////////localStream.setVideoProfile(cameraVideoProfile);

  localStream.init(function () {
    console.log("getUserMedia successfully");
    // TODO: add check for other streams. play local stream full size if alone in channel
    localStream.play('local-video'); // play the given stream within the local-video div

    // publish local stream
    client.publish(localStream, function (err) {
      console.log("[ERROR] : publish local stream error: " + err);
    });

    enableUiControls(localStream); // move after testing
    localStreams.camera.stream = localStream; // keep track of the camera stream for later
  }, function (err) {
    console.log("[ERROR] : getUserMedia failed", err);
  });
}

// SCREEN SHARING
function initScreenShare() {
  screenClient.init(agoraAppId, function () {
    console.log("AgoraRTC screenClient initialized");
    joinChannelAsScreenShare();
    screenShareActive = true;
    // TODO: add logic to swap button
  }, function (err) {
    console.log("[ERROR] : AgoraRTC screenClient init failed", err);
  });
}

function joinChannelAsScreenShare() {
  //var token = generateToken();
  //var userID = null; // set to null to auto generate uid on successfull connection

  var token = $('#apptsscreensharetoken').val()
  var userID = $('#apptsscreenshareuid').val();

  screenClient.join(token, channelName, userID, function (uid) {
    localStreams.screen.id = uid;  // keep track of the uid of the screen stream.

    // Create the stream for screen sharing.
    var screenStream = AgoraRTC.createStream({
      streamID: uid,
      audio: false, // Set the audio attribute as false to avoid any echo during the call.
      video: false,
      screen: true, // screen stream
      extensionId: 'minllpmhdgpndnkomcoccfekfegnlikg', // Google Chrome:
      mediaSource: 'screen', // Firefox: 'screen', 'application', 'window' (select one)
    });
    screenStream.setScreenProfile(screenVideoProfile); // set the profile of the screen
    screenStream.init(function () {
      console.log("getScreen successful");
      localStreams.screen.stream = screenStream; // keep track of the screen stream
      $("#screen-share-btn").prop("disabled", false); // enable button
      screenClient.publish(screenStream, function (err) {
        console.log("[ERROR] : publish screen stream error: " + err);
      });
    }, function (err) {
      console.log("[ERROR] : getScreen failed", err);
      localStreams.screen.id = ""; // reset screen stream id
      localStreams.screen.stream = {}; // reset the screen stream
      screenShareActive = false; // resest screenShare
      toggleScreenShareBtn(); // toggle the button icon back (will appear disabled)
    });
  }, function (err) {
    console.log("[ERROR] : join channel as screen-share failed", err);
  });

  screenClient.on('stream-published', function (evt) {
    console.log("Publish screen stream successfully");
    localStreams.camera.stream.disableVideo(); // disable the local video stream (will send a mute signal)
    localStreams.camera.stream.stop(); // stop playing the local stream
    // TODO: add logic to swap main video feed back from container
    remoteStreams[mainStreamId].stop(); // stop the main video stream playback
    addRemoteStreamMiniView(remoteStreams[mainStreamId]); // send the main video stream to a container
    // localStreams.screen.stream.play('full-screen-video'); // play the screen share as full-screen-video (vortext effect?)
    $("#video-btn").prop("disabled", true); // disable the video button (as cameara video stream is disabled)
  });

  screenClient.on('stopScreenSharing', function (evt) {
    console.log("screen sharing stopped", err);
  });
}

function stopScreenShare() {
  localStreams.screen.stream.disableVideo(); // disable the local video stream (will send a mute signal)
  localStreams.screen.stream.stop(); // stop playing the local stream
  localStreams.camera.stream.enableVideo(); // enable the camera feed
  localStreams.camera.stream.play('local-video'); // play the camera within the full-screen-video div
  $("#video-btn").prop("disabled", false);
  screenClient.leave(function () {
    screenShareActive = false;
    console.log("screen client leaves channel");
    $("#screen-share-btn").prop("disabled", false); // enable button
    screenClient.unpublish(localStreams.screen.stream); // unpublish the screen client
    localStreams.screen.stream.close(); // close the screen client stream
    localStreams.screen.id = ""; // reset the screen id
    localStreams.screen.stream = {}; // reset the stream obj
  }, function (err) {
    console.log("client leave failed ", err); //error handling
  });
}

// REMOTE STREAMS UI
function addRemoteStreamMiniView(remoteStream) {
  var streamId = remoteStream.getId();
  // append the remote stream template to #remote-streams
  $('#remote-streams').append(
    $('<div/>', { 'id': streamId + '_container', 'class': 'remote-stream-container col' }).append(
      $('<div/>', { 'id': streamId + '_mute', 'class': 'mute-overlay' }).append(
        $('<i/>', { 'class': 'fas fa-microphone-slash' })
      ),
      $('<div/>', { 'id': streamId + '_no-video', 'class': 'no-video-overlay text-center' }).append(
        $('<i/>', { 'class': 'fas fa-user' })
      ),
      $('<div/>', { 'id': 'agora_remote_' + streamId, 'class': 'remote-video' })
    )
  );
  remoteStream.play('agora_remote_' + streamId);

  var containerId = '#' + streamId + '_container';
  $(containerId).dblclick(function () {
    // play selected container as full screen - swap out current full screen stream
    remoteStreams[mainStreamId].stop(); // stop the main video stream playback
    addRemoteStreamMiniView(remoteStreams[mainStreamId]); // send the main video stream to a container
    $(containerId).empty().remove(); // remove the stream's miniView container
    remoteStreams[streamId].stop() // stop the container's video stream playback
    remoteStreams[streamId].play('full-screen-video'); // play the remote stream as the full screen video
    mainStreamId = streamId; // set the container stream id as the new main stream id
  });
}

function leaveChannel() {

  if (screenShareActive) {
    stopScreenShare();
  }

  client.leave(function () {
    console.log("client leaves channel");
    localStreams.camera.stream.stop() // stop the camera stream playback
    client.unpublish(localStreams.camera.stream); // unpublish the camera stream
    localStreams.camera.stream.close(); // clean up and close the camera stream
    $("#remote-streams").empty() // clean up the remote feeds
    //disable the UI elements
    $("#mic-btn").prop("disabled", true);
    $("#video-btn").prop("disabled", true);
    $("#screen-share-btn").prop("disabled", true);
    $("#exit-btn").prop("disabled", true);
    // hide the mute/no-video overlays
    toggleVisibility("#mute-overlay", false);
    toggleVisibility("#no-local-video", false);
  }, function (err) {
    console.log("client leave failed ", err); //error handling
  });
}

// use tokens for added security
function generateToken() {
  return temptoken;
  //return null; // TODO: add a token generation
}


// UI buttons
function enableUiControls(localStream) {
  console.log('enable ui controls called');
  $("#mic-btn").prop("disabled", false);
  $("#video-btn").prop("disabled", false);
  $("#screen-share-btn").prop("disabled", false);
  $("#exit-btn").prop("disabled", false);

  $("#mic-btn").click(function () {
    toggleMic(localStream);
    console.log('enable ui ctls mic-btn click', localStream);
  });

  $("#video-btn").click(function () {
    toggleVideo(localStream);
  });

  $("#screen-share-btn").click(function () {
    toggleScreenShareBtn(); // set screen share button icon
    $("#screen-share-btn").prop("disabled", true); // disable the button on click
    if (screenShareActive) {
      stopScreenShare();
    } else {
      initScreenShare();
    }
  });

  $("#exit-btn").click(function () {
    console.log("so sad to see you leave the channel");
    leaveChannel();
  });

  // keyboard listeners 
  $(document).keypress(function (e) {
    switch (e.key) {
      case "m":
        console.log("squick toggle the mic");
        toggleMic(localStream);
        break;
      case "v":
        console.log("quick toggle the video");
        toggleVideo(localStream);
        break;
      case "s":
        console.log("initializing screen share");
        toggleScreenShareBtn(); // set screen share button icon
        $("#screen-share-btn").prop("disabled", true); // disable the button on click
        if (screenShareActive) {
          stopScreenShare();
        } else {
          initScreenShare();
        }
        break;
      case "q":
        console.log("so sad to see you quit the channel");
        leaveChannel();
        break;
      default:  // do nothing
    }

    // (for testing) 
    if (e.key === "r") {
      window.history.back(); // quick reset
    }
  });
}

function toggleBtn(btn) {
  btn.toggleClass('btn-dark').toggleClass('btn-danger');
}

function toggleScreenShareBtn() {
  $('#screen-share-btn').toggleClass('btn-danger');
  $('#screen-share-icon').toggleClass('fa-share-square').toggleClass('fa-times-circle');
}

function toggleVisibility(elementID, visible) {
  if (visible) {
    $(elementID).attr("style", "display:block");
  } else {
    $(elementID).attr("style", "display:none");
  }
}

function toggleMic(localStream) {
  toggleBtn($("#mic-btn")); // toggle button colors
  $("#mic-icon").toggleClass('fa-microphone').toggleClass('fa-microphone-slash'); // toggle the mic icon
  if ($("#mic-icon").hasClass('fa-microphone')) {
    localStream.enableAudio(); // enable the local mic
    toggleVisibility("#mute-overlay", false); // hide the muted mic icon
  } else {
    localStream.disableAudio(); // mute the local mic
    toggleVisibility("#mute-overlay", true); // show the muted mic icon
  }
}

function toggleVideo(localStream) {
  toggleBtn($("#video-btn")); // toggle button colors
  $("#video-icon").toggleClass('fa-video').toggleClass('fa-video-slash'); // toggle the video icon
  if ($("#video-icon").hasClass('fa-video')) {
    localStream.enableVideo(); // enable the local video
    toggleVisibility("#no-local-video", false); // hide the user icon when video is enabled
  } else {
    localStream.disableVideo(); // disable the local video
    toggleVisibility("#no-local-video", true); // show the user icon when video is disabled
  }
}