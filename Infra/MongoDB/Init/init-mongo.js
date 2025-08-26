db = db.getSiblingDB("simuladorcaixa");

db.createUser({
  user: "simuladorUser",
  pwd: "senhaForte123",
  roles: [
    {
      role: "readWrite",
      db: "simuladorcaixa"
    }
  ]
});