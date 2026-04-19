[
  {
    "cursor": "4hIXc6h9sJJzMmRja2Z2YzczZjk1OGRn",
    "service": {
      "autoDeploy": "yes",
      "autoDeployTrigger": "commit",
      "branch": "main",
      "createdAt": "2026-04-19T22:58:17.456308Z",
      "dashboardUrl": "https://dashboard.render.com/web/srv-d7ils2dckfvc73f958dg",
      "id": "srv-d7ils2dckfvc73f958dg",
      "name": "server",
      "notifyOnFail": "default",
      "ownerId": "tea-d7i0q3lckfvc73escfq0",
      "repo": "https://github.com/Miri327/TodoList",
      "rootDir": "./server",
      "serviceDetails": {
        "buildPlan": "starter",
        "cache": {
          "profile": "no-cache"
        },
        "env": "docker",
        "envSpecificDetails": {
          "dockerCommand": "",
          "dockerContext": ".",
          "dockerfilePath": "./Dockerfile"
        },
        "healthCheckPath": "",
        "ipAllowList": [
          {
            "cidrBlock": "0.0.0.0/0",
            "description": "everywhere"
          }
        ],
        "maintenanceMode": {
          "enabled": false,
          "uri": ""
        },
        "numInstances": 1,
        "openPorts": null,
        "plan": "free",
        "previews": {
          "generation": "off"
        },
        "pullRequestPreviewsEnabled": "no",
        "region": "singapore",
        "runtime": "docker",
        "sshAddress": "srv-d7ils2dckfvc73f958dg@ssh.singapore.render.com",
        "url": "https://server-d6fx.onrender.com"
      },
      "slug": "server-d6fx",
      "suspended": "not_suspended",
      "suspenders": [],
      "type": "web_service",
      "updatedAt": "2026-04-19T23:18:09.833748Z"
    }
  },
  {
    "cursor": "4hIXc6h9sJI1MW5sazFtYzczYTBnYmUw",
    "service": {
      "autoDeploy": "yes",
      "autoDeployTrigger": "commit",
      "branch": "main",
      "createdAt": "2026-04-19T22:09:10.488723Z",
      "dashboardUrl": "https://dashboard.render.com/static/srv-d7il51nlk1mc73a0gbe0",
      "environmentId": "evm-d7il51f7f7vs7399s7pg",
      "id": "srv-d7il51nlk1mc73a0gbe0",
      "name": "client",
      "notifyOnFail": "default",
      "ownerId": "tea-d7i0q3lckfvc73escfq0",
      "repo": "https://github.com/Miri327/TodoList",
      "rootDir": "./client",
      "serviceDetails": {
        "buildCommand": "npm run build",
        "buildPlan": "starter",
        "ipAllowList": [
          {
            "cidrBlock": "0.0.0.0/0",
            "description": "everywhere"
          }
        ],
        "previews": {
          "generation": "off"
        },
        "publishPath": "build",
        "pullRequestPreviewsEnabled": "no",
        "url": "https://client-69ed.onrender.com"
      },
      "slug": "client-69ed",
      "suspended": "not_suspended",
      "suspenders": [],
      "type": "static_site",
      "updatedAt": "2026-04-19T23:27:03.589783Z"
    }
  }
]