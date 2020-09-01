const prBody = danger.github.pr.body
const adoLink_regex = /(AB#)[0-91]+/
      
if ((prBody.match(adoLink_regex)===null)) {
  fail('Please provide a link to the relevant card in the Pull Request description 💩')
}

if (!(prBody.includes('Functional change') || prBody.includes('Bug fix'))) {
  fail('Please specify the type of change in the Pull Request description 💩')
}

if (!(prBody.includes('Solution'))) {
  fail('Please specify the solution in the Pull Request description 💩')
}
