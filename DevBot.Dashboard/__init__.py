from flask import Flask, redirect, render_template, url_for, request
import json
from flask_discord import DiscordOAuth2Session, requires_authorization

app = Flask(__name__)

with open("./settings.json", "r") as f:
    config = json.loads(f.read())

app.secret_key = config["flask"]["appsecret"]

app.config["DEBUG"] = True
app.config["DISCORD_CLIENT_ID"] = config["discord"]["clientid"]
app.config["DISCORD_CLIENT_SECRET"] = config["discord"]["clientsecret"]
app.config["DISCORD_REDIRECT_URI"] = config["discord"]["redirect"]

discord = DiscordOAuth2Session(app)

@app.route("/discord/login")
def _discord_login():
    return discord.create_session()

@app.route("/discord/logout")
@requires_authorization
def _discord_logout():

    discord.revoke()
    return redirect("/")


@app.route("/")
def index():

    return render_template("index.html")

app.run('localhost', 443, ssl_context='adhoc')