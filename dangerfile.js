const LINE_LIMIT = 500

var totalLines = danger.github.pr.additions + danger.github.pr.deletions
if (!danger.github.pr.body.includes('[IGNORE_LINE_LIMIT]') && totalLines > LINE_LIMIT) {
  fail(`Pull Request is too long (over ${LINE_LIMIT} lines). You have ${totalLines} lines 💩`)
} else if (totalLines > LINE_LIMIT * 0.9) {
  warn(`Pull Request is almost over ${LINE_LIMIT} lines. You have ${totalLines} lines ⚠️`)
} else {
  message(`You have changed ${totalLines} lines towards our PR limit.`)
}

if (!(danger.github.pr.body.includes('amaabca.visualstudio.com') ||
    danger.github.pr.body.includes('dev.azure.com') ||
    danger.github.pr.body.includes('app.opsgenie.com'))) {
  fail('Please provide a link to the relevant card in the Pull Request description 💩')
}
