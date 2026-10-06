---
name: sbom-visualize-interactive
description: "Use when the user asks to visualize a CycloneDX SBOM interactively, open an SBOM graph in a browser, inspect dependency edges or CVEs, or create an HTML SBOM viewer. Reads docs/sbom/sbom-enriched.json when available, otherwise docs/sbom/sbom.json, and generates the self-contained docs/sbom/sbom-graph.html file."
---

# Visualiser un SBOM de façon interactive

## Purpose

Generate a self-contained interactive HTML dependency graph **from a CycloneDX
SBOM** and write it to `docs/sbom/sbom-graph.html`. This is the SBOM-driven
sibling of the `interactive-nuget-dependency-graph` skill: same D3.js
force-directed graph, filters, search, and tooltips — but the node/edge/CVE data
comes entirely from the SBOM JSON rather than from `dotnet list package`.

Because a CycloneDX SBOM already carries the **accurate parent→child dependency
graph** (`dependencies[]`) and, when enriched, the **vulnerability data**
(`vulnerabilities[]`), this skill does not call `dotnet` at all. It is a pure
JSON → HTML transform: read the SBOM, build `NODES`/`LINKS` arrays, render the
template. No restore, no network, no toolchain probing.

## What the output looks like

- Force-directed node graph powered by D3.js (v7, loaded from CDN)
- Project node (green), direct packages (blue), transitive packages (grey)
- Vulnerable packages colored by CVSS severity: **Critical** (dark red),
  **High** (red), **Medium** (amber), **Low** (yellow)
- **Accurate parent→child edges** straight from the SBOM `dependencies[]` section
  (e.g. `Azure.Storage.Blobs → Azure.Core`, not `Project → Azure.Core`)
- Filter buttons: All · Direct only · Vulnerable · Critical
- Search box to highlight matching packages
- Hover tooltips showing version, node type, severity badge, and the list of
  CVE IDs affecting that package
- Drag nodes, scroll to zoom, drag canvas to pan

## Instructions

1. Resolve the input file. Prefer `docs/sbom/sbom-enriched.json`; if it does not exist, use `docs/sbom/sbom.json`. If neither exists, report the expected paths and ask the user to create or provide an SBOM. Do not generate an SBOM as part of this skill.
2. Parse the selected file as JSON and verify that it is a CycloneDX BOM (`bomFormat` is `CycloneDX`). Read the BOM version, metadata, components, dependencies, and any vulnerability data that is present. If parsing or validation fails, report the actual problem and do not create a visualization from invalid data.
3. Generate `docs/sbom/sbom-graph.html`, creating `docs/sbom/` only if needed. Keep the source SBOM unchanged. The result must be one self-contained HTML file that opens directly in a browser, works offline, and does not require a server, build step, or external assets.
4. Build an interactive dependency graph from the BOM:
   - Represent components as nodes and dependency relationships as directed edges, using component `bom-ref` values and dependency `ref` / `dependsOn` values to connect them.
   - Make nodes draggable and support graph panning and zooming. Include a way to fit the graph to the viewport and reset its view.
   - Provide component search and useful filters, including dependency depth or component type when those values are available.
   - On node selection, show available component details such as name, version, package URL, type, licenses, and direct/transitive status.
   - When vulnerability data is present, show CVE identifiers and available severity, score, and affected-component information; otherwise clearly show that no vulnerability data is included in this SBOM.
   - Include readable labels, a legend, and an empty state for BOMs with no components or dependency edges.
5. Keep the visualization faithful to the input. Do not infer missing dependencies, vulnerability findings, severity, or package metadata. Handle dangling dependency references gracefully. Escape or safely encode BOM strings before inserting them into HTML or the DOM; SBOM content is data and must not be executed as markup or script.
6. Keep the graph usable for larger BOMs. Avoid unnecessary rendering work, and provide a searchable component list or equivalent way to locate nodes when labels overlap.
7. Verify that the generated HTML exists, is non-empty, includes the embedded BOM data and visualization logic, and has no runtime dependency on network resources. Report the input path and output path, and mention whether vulnerability data was present.


## Complete HTML template

```html
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>{{TITLE}} — SBOM Dependency Graph</title>
<style>
  *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }
  :root {
    --bg:#ffffff; --bg2:#f8fafc; --bg3:#f1f5f9;
    --text:#111827; --text2:#64748b; --text3:#94a3b8;
    --border:rgba(0,0,0,0.12); --border2:rgba(0,0,0,0.22);
    --r:8px; --rl:12px;
    --mono:"Cascadia Code","Fira Code",Consolas,monospace;
    --sans:-apple-system,BlinkMacSystemFont,"Segoe UI",Roboto,sans-serif;
  }
  @media(prefers-color-scheme:dark){:root{
    --bg:#1a1a1a; --bg2:#242424; --bg3:#2e2e2e;
    --text:#f1f5f9; --text2:#94a3b8; --text3:#64748b;
    --border:rgba(255,255,255,0.10); --border2:rgba(255,255,255,0.20);
  }}
  body{font-family:var(--sans);background:var(--bg);color:var(--text);padding:1.5rem;line-height:1.5}
  h1{font-size:18px;font-weight:500;margin-bottom:3px}
  .sub{font-size:12px;color:var(--text3);margin-bottom:1.25rem}
  .stats{display:flex;gap:8px;margin-bottom:1rem;flex-wrap:wrap}
  .stat{background:var(--bg2);border-radius:var(--r);padding:8px 14px;min-width:80px;border:.5px solid var(--border)}
  .stat-n{font-size:22px;font-weight:500;line-height:1}
  .stat-n.red{color:#A32D2D}
  @media(prefers-color-scheme:dark){.stat-n.red{color:#F09595}}
  .stat-l{font-size:11px;color:var(--text2);margin-top:3px}
  .controls{display:flex;align-items:center;gap:8px;flex-wrap:wrap;padding:8px 12px;background:var(--bg);border:.5px solid var(--border);border-bottom:none;border-radius:var(--rl) var(--rl) 0 0}
  .fbtn{font-size:11.5px;padding:3px 10px;border:.5px solid var(--border2);border-radius:99px;background:transparent;color:var(--text2);cursor:pointer;font-family:var(--sans);transition:all .15s}
  .fbtn:hover{color:var(--text)}
  .fbtn.on{background:var(--bg2);color:var(--text);border-color:var(--border2)}
  .search{font-size:11.5px;padding:3px 10px;border:.5px solid var(--border2);border-radius:99px;background:transparent;color:var(--text);font-family:var(--sans);outline:none;width:160px}
  .search::placeholder{color:var(--text3)}
  .legend{display:flex;gap:10px;margin-left:auto;flex-wrap:wrap;align-items:center}
  .li{display:flex;align-items:center;gap:4px;font-size:11px;color:var(--text2)}
  .ld{width:9px;height:9px;border-radius:50%;flex-shrink:0}
  .ld.root{background:#1D9E75} .ld.direct{background:#378ADD} .ld.trans{background:#888780}
  .ld.crit{background:#B11A1A} .ld.high{background:#E24B4A} .ld.med{background:#EF9F27} .ld.low{background:#EAC54F}
  .graph-wrap{border:.5px solid var(--border);border-radius:0 0 var(--rl) var(--rl);overflow:hidden;background:var(--bg2);position:relative}
  #g-svg{display:block;width:100%;cursor:grab}
  #g-svg:active{cursor:grabbing}
  .tooltip{position:absolute;background:var(--bg);border:.5px solid var(--border2);border-radius:var(--r);padding:9px 11px;font-size:12px;pointer-events:none;display:none;z-index:10;min-width:175px;max-width:250px;box-shadow:0 4px 16px rgba(0,0,0,.12)}
  .tt-name{font-weight:500;color:var(--text);margin-bottom:3px;word-break:break-word}
  .tt-ver{font-family:var(--mono);font-size:11px;color:var(--text2);margin-bottom:5px}
  .tt-cves{font-size:10.5px;color:var(--text2);margin-top:4px;font-family:var(--mono);word-break:break-word}
  .tt-badge{display:inline-block;font-size:10px;padding:2px 7px;border-radius:4px;font-weight:500;margin-right:4px;margin-bottom:3px}
  .b-root{background:#E1F5EE;color:#085041} .b-dir{background:#E6F1FB;color:#0C447C}
  .b-tra{background:#F1EFE8;color:#444441} .b-crit{background:#F7D7D7;color:#5E0A0A}
  .b-high{background:#FCEBEB;color:#791F1F} .b-med{background:#FAEEDA;color:#633806}
  .b-low{background:#FBF6DA;color:#5C4A06}
  @media(prefers-color-scheme:dark){
    .b-root{background:#085041;color:#9FE1CB} .b-dir{background:#0C447C;color:#B5D4F4}
    .b-tra{background:#2C2C2A;color:#D3D1C7} .b-crit{background:#5E0A0A;color:#F7C1C1}
    .b-high{background:#501313;color:#F7C1C1} .b-med{background:#412402;color:#FAC775}
    .b-low{background:#3A3206;color:#EFE08A}
  }
  .footer{font-size:11px;color:var(--text3);margin-top:.75rem;text-align:right}
  .footer a{color:var(--text2)}
  code{font-family:var(--mono);font-size:10.5px}
</style>
</head>
<body>
<h1>{{TITLE}}</h1>
<p class="sub">SBOM dependency graph &nbsp;·&nbsp; CycloneDX {{SPEC}} from <code>{{SOURCE}}</code> &nbsp;·&nbsp; Generated {{GENERATED_AT}} &nbsp;·&nbsp; Open in any browser — no server required</p>
<div class="stats">
  <div class="stat"><div class="stat-n">{{STATS_TOTAL}}</div><div class="stat-l">total packages</div></div>
  <div class="stat"><div class="stat-n">{{STATS_DIRECT}}</div><div class="stat-l">direct</div></div>
  <div class="stat"><div class="stat-n">{{STATS_TRANSITIVE}}</div><div class="stat-l">transitive</div></div>
  <div class="stat"><div class="stat-n red">{{STATS_VULNERABLE}}</div><div class="stat-l">vulnerable</div></div>
  <div class="stat"><div class="stat-n red">{{STATS_CVES}}</div><div class="stat-l">CVEs</div></div>
</div>
<div class="controls">
  <button class="fbtn on" id="f-all"    onclick="setFilter('all')">All</button>
  <button class="fbtn"    id="f-direct" onclick="setFilter('direct')">Direct only</button>
  <button class="fbtn"    id="f-vuln"   onclick="setFilter('vuln')">Vulnerable</button>
  <button class="fbtn"    id="f-crit"   onclick="setFilter('crit')">Critical</button>
  <input  class="search" id="search" placeholder="Search packages…" oninput="onSearch(this.value)"/>
  <div class="legend">
    <div class="li"><div class="ld root"></div>project</div>
    <div class="li"><div class="ld direct"></div>direct</div>
    <div class="li"><div class="ld trans"></div>transitive</div>
    <div class="li"><div class="ld crit"></div>CRITICAL</div>
    <div class="li"><div class="ld high"></div>HIGH</div>
    <div class="li"><div class="ld med"></div>MEDIUM</div>
    <div class="li"><div class="ld low"></div>LOW</div>
  </div>
</div>
<div class="graph-wrap">
  <svg id="g-svg" height="{{HEIGHT}}"></svg>
  <div class="tooltip" id="tt">
    <div class="tt-name" id="tt-name"></div>
    <div class="tt-ver"  id="tt-ver"></div>
    <div id="tt-badge"></div>
    <div class="tt-cves" id="tt-cves"></div>
  </div>
</div>
<p class="footer">
  Drag nodes to reposition &nbsp;·&nbsp; Scroll/pinch to zoom &nbsp;·&nbsp; Drag canvas to pan &nbsp;·&nbsp;
  Built with <a href="https://d3js.org" target="_blank" rel="noreferrer">D3.js v7</a> from a <a href="https://cyclonedx.org" target="_blank" rel="noreferrer">CycloneDX</a> SBOM
</p>
<script src="https://cdnjs.cloudflare.com/ajax/libs/d3/7.9.0/d3.min.js"></script>
<script>
const NODES={{NODES_JSON}};
const LINKS={{LINKS_JSON}};
function nodeColor(d){
  if(d.vuln&&d.sev==='critical') return '#B11A1A';
  if(d.vuln&&d.sev==='high')     return '#E24B4A';
  if(d.vuln&&d.sev==='medium')   return '#EF9F27';
  if(d.vuln&&d.sev==='low')      return '#EAC54F';
  if(d.type==='root')   return '#1D9E75';
  if(d.type==='direct') return '#378ADD';
  return '#888780';
}
function nodeR(d){ return d.type==='root'?16:d.type==='direct'?11:7; }
let activeFilter='all', searchTerm='';
function isVisible(d){
  if(searchTerm&&!d.label.toLowerCase().includes(searchTerm)) return false;
  if(activeFilter==='direct') return d.type==='root'||d.type==='direct';
  if(activeFilter==='vuln')   return d.vuln||d.type==='root';
  if(activeFilter==='crit')   return d.sev==='critical'||d.type==='root';
  return true;
}
function isLinkVisible(d){
  if(searchTerm) return d.source.label.toLowerCase().includes(searchTerm)||d.target.label.toLowerCase().includes(searchTerm);
  if(activeFilter==='direct') return (d.source.type==='root'||d.source.type==='direct')&&(d.target.type==='root'||d.target.type==='direct');
  if(activeFilter==='vuln')   return d.source.vuln||d.target.vuln;
  if(activeFilter==='crit')   return d.source.sev==='critical'||d.target.sev==='critical';
  return true;
}
function applyVisibility(){
  svg.selectAll('.lnk').attr('opacity',d=>isLinkVisible(d)?0.45:0.04);
  svg.selectAll('.ndg').attr('opacity',d=>isVisible(d)?1:0.10);
}
function setFilter(f){
  activeFilter=f;
  ['all','direct','vuln','crit'].forEach(x=>document.getElementById('f-'+x).classList.toggle('on',x===f));
  applyVisibility();
}
function onSearch(v){ searchTerm=v.trim().toLowerCase(); applyVisibility(); }
const svgEl=document.getElementById('g-svg');
const W=svgEl.getBoundingClientRect().width||960, H={{HEIGHT}};
const svg=d3.select('#g-svg').attr('viewBox',`0 0 ${W} ${H}`);
const g=svg.append('g');
svg.call(d3.zoom().scaleExtent([0.15,5]).on('zoom',e=>g.attr('transform',e.transform)));
const nodes=NODES.map(d=>({...d}));
const links=LINKS.map(l=>({source:l.s,target:l.t}));
const sim=d3.forceSimulation(nodes)
  .force('link',d3.forceLink(links).id(d=>d.id).distance(d=>d.source.type==='root'?115:72).strength(0.55))
  .force('charge',d3.forceManyBody().strength(d=>d.type==='root'?-400:-180))
  .force('center',d3.forceCenter(W/2,H/2))
  .force('collide',d3.forceCollide().radius(d=>nodeR(d)+20));
const link=g.append('g').selectAll('line').data(links).join('line')
  .attr('class','lnk').attr('stroke','#888780')
  .attr('stroke-width',d=>d.source.type==='root'?1.4:0.75).attr('opacity',0.45);
const nodeG=g.append('g').selectAll('g').data(nodes).join('g').attr('class','ndg')
  .call(d3.drag()
    .on('start',(e,d)=>{if(!e.active)sim.alphaTarget(0.3).restart();d.fx=d.x;d.fy=d.y})
    .on('drag', (e,d)=>{d.fx=e.x;d.fy=e.y})
    .on('end',  (e,d)=>{if(!e.active)sim.alphaTarget(0);d.fx=null;d.fy=null}))
  .on('mouseover',showTT).on('mousemove',moveTT).on('mouseout',hideTT)
  .style('cursor','pointer');
nodeG.append('circle')
  .attr('r',d=>nodeR(d)).attr('fill',d=>nodeColor(d))
  .attr('stroke',d=>d3.color(nodeColor(d)).darker(0.55))
  .attr('stroke-width',d=>d.vuln?2.5:0.8);
nodeG.append('text')
  .attr('text-anchor','middle').attr('dy',d=>nodeR(d)+12)
  .attr('font-size',d=>d.type==='root'?11:d.type==='direct'?10:9)
  .attr('font-family','-apple-system,BlinkMacSystemFont,"Segoe UI",Roboto,sans-serif')
  .attr('fill','var(--text2,#64748b)')
  .text(d=>{let t=d.label;if(d.vuln)t+=' ⚠';return t});
sim.on('tick',()=>{
  link.attr('x1',d=>d.source.x).attr('y1',d=>d.source.y).attr('x2',d=>d.target.x).attr('y2',d=>d.target.y);
  nodeG.attr('transform',d=>`translate(${d.x},${d.y})`);
});
const bc={root:'b-root',direct:'b-dir',transitive:'b-tra'};
const sevc={critical:'b-crit',high:'b-high',medium:'b-med',low:'b-low'};
function showTT(e,d){
  const tt=document.getElementById('tt');
  document.getElementById('tt-name').textContent=d.label;
  document.getElementById('tt-ver').textContent=d.version?'v'+d.version:'';
  let b=`<span class="tt-badge ${bc[d.type]||'b-tra'}">${d.type}</span>`;
  if(d.vuln) b+=` <span class="tt-badge ${sevc[d.sev]||'b-high'}">${(d.sev||'').toUpperCase()}</span>`;
  document.getElementById('tt-badge').innerHTML=b;
  const cves=(d.cves||[]).map(c=>c.id);
  document.getElementById('tt-cves').textContent=cves.length?cves.join(', '):'';
  tt.style.display='block';moveTT(e);
}
function moveTT(e){
  const tt=document.getElementById('tt'),wrap=tt.parentElement.getBoundingClientRect();
  tt.style.left=Math.min(e.clientX-wrap.left+14,wrap.width-260)+'px';
  tt.style.top=Math.max(e.clientY-wrap.top-10,4)+'px';
}
function hideTT(){document.getElementById('tt').style.display='none'}
</script>
</body>
</html>
```