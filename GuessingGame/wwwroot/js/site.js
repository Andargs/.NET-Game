function showPreview(event){
    if(event.target.files.length > 0){
      var img = new Image();
      img.src = URL.createObjectURL(event.target.files[0]);
      var canvas = document.getElementById("canv");
      var context = canvas.getContext("2d");
      img.onload=function()
      {
        context.drawImage(img, 0, 0, 740, 420);
      }
    }
}
canvas = document.getElementById('canv');
var ctx = canvas.getContext('2d');
var bound = canvas.getBoundingClientRect();

// last known position
var pos = { x: 0, y: 0 };

document.addEventListener('mousemove', draw);
document.addEventListener('mousedown', setPosition);
document.addEventListener('mouseenter', setPosition);

// new position from mouse event
function setPosition(e) {
  pos.x = e.offsetX;
  pos.y = e.offsetY;
}

function draw(e) {
  // mouse left button must be pressed
  if (e.buttons !== 1) return;
  if(e.clientX > bound.right-1) return;
  if(e.clientY > 651) return;
  if(e.clientX < bound.left) return;
  if(e.clientY < 235) return;

  ctx.beginPath(); // begin

  ctx.lineWidth = 5;
  ctx.lineCap = 'round';
  ctx.strokeStyle = 'rgba(255, 10, 255)';

  ctx.moveTo(pos.x, pos.y); // from
  setPosition(e);
  ctx.lineTo(pos.x, pos.y); // to
   // check if mouse coordinates are off canvas and if they are stop drawing!
  ctx.stroke(); // draw it!
}

function showCoords(event) {
  var x = event.offsetX;
  var y = event.offsetY;
  var cx = event.clientX;
  var cy = event.clientY;
  var coords = "offsetX coords: " + x + ", offsetY coords: " + y + ", clientX: "+ cx+ ", clientY: "+ cy;
  document.getElementById("demo").innerHTML = "Tile clicked on!";
  document.getElementById("X").value = x;
  document.getElementById("Y").value = y;
  document.getElementById("cordi").innerHTML = coords;
}

function send() {
  console.log("reached!");
  var canvas = document.getElementById("canv");
  var image = canvas.toDataURL("image/png");
  image = image.replace('data:image/png;base64,', '');
  $('#imageData').val(image);
}

// This function is used in CreateGame.cshtml. 
// This function keeps the "number of players" input field hidden if you select
// the game mode singleplayer or twoplayer because then the number of players will be known
// and calculated here and the value of the hidden "number of players" will be set to 1 or 2 (players).
// If one of the multiplayer game modes is shown then the "number of players" input field will appear.
function calculate_num_players(id){
    if(id == "sp"){
        document.getElementById("label_num_players").style.visibility = "hidden";
        document.getElementById("num_players").style.visibility = "hidden";
        document.getElementById("num_players").value=1; //set to 1 because singleplayer mode.
    }else if(id == "tp"){
        document.getElementById("label_num_players").style.visibility = "hidden";
        document.getElementById("num_players").style.visibility = "hidden";
        document.getElementById("num_players").value=2; //set to 2 because twoplayer mode.
    }else{
        document.getElementById("label_num_players").style.visibility = "visible";
        document.getElementById("num_players").style.visibility = "visible";
    }
}