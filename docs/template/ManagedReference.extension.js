// The modern template renders enum-field remarks immediately after their
// summaries without a distinct container. Wrap them so they can be styled
// consistently with remarks on types and other members.
exports.preTransform = function (model) {
  if (model.type?.toLowerCase() !== "enum") {
    return model;
  }

  for (const child of model.children ?? []) {
    if (child.remarks) {
      child.remarks = `
        <aside class="api-remarks api-remarks--enum" aria-label="Remarks">
          <div class="api-remarks__title">Remarks</div>
          ${child.remarks}
        </aside>`;
    }
  }

  return model;
};

exports.postTransform = function (model) {
  return model;
};
